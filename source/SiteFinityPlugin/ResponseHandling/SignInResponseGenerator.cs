using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using IdentityServer.SiteFinity.Services;
using IdentityServer.SiteFinity.Utilities;
using IdentityServer.SiteFinity.Validation;
using Thinktecture.IdentityServer.Core.Logging;

namespace IdentityServer.SiteFinity.ResponseHandling
{
    public class SignInResponseGenerator
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly HttpUtility _httpUtility;
        private readonly SimpleWebTokenParser _simpleWebTokenParser;

        public SignInResponseGenerator(HttpUtility httpUtility, SimpleWebTokenParser simpleWebTokenParser)
        {
            _httpUtility = httpUtility;
            _simpleWebTokenParser = simpleWebTokenParser;
        }


        public async Task<IHttpActionResult> GenerateResponseAsync(SignInRequestMessage message, SignInValidationResult result,HttpRequestMessage request)
        {
            Logger.Info("Creating SiteFinity signin response");

            var principal = new ClaimsPrincipal(result.Subject);
            var identity = ClaimsPrincipal.PrimaryIdentitySelector(principal.Identities);

            var token = await CreateToken(identity.Name, identity.Claims, result);

            NameValueCollection queryString;
            if (!String.IsNullOrEmpty(result.ReplyUrl))
            {
                string path;
                var idx = result.ReplyUrl.IndexOf('?');
                if (idx != -1)
                {
                    path = result.ReplyUrl.Substring(0, idx);
                    queryString = ParseQueryString(result.ReplyUrl.Substring(idx + 1));
                }
                else
                {
                    path = result.ReplyUrl;
                    queryString = new NameValueCollection();
                }
                WrapSWT(queryString, token, message.Deflate);
                path = String.Concat(path, ToQueryString(queryString));
                var uri = new Uri(new Uri(result.Realm), path);

                var redirectResult = new RedirectResult(uri,request);
                return redirectResult;
            }

            queryString = new NameValueCollection();
            WrapSWT(queryString, token, message.Deflate);

            var content = new StringContent(ToQueryString(queryString, false), Encoding.UTF8,"application/x-www-form-urlencoded");
            var responseMessage = request.CreateResponse(HttpStatusCode.OK,content);
            return new ResponseMessageResult(responseMessage);

        }

        private async Task<SimpleWebToken> CreateToken(string name,IEnumerable<Claim> claims, SignInValidationResult result)
        {

            var key = this.HexToByte(result.SiteFinityRelyingParty.Key);

            var sb = new StringBuilder();
            foreach (var c in claims)
            {
                sb.AppendFormat("{0}={1}&", _httpUtility.UrlEncode(c.Type), _httpUtility.UrlEncode(c.Value));
            }

            sb.AppendFormat("{0}={1}&", _httpUtility.UrlEncode(SitefinityClaimTypes.StsType), "wa");
            sb.AppendFormat("{0}={1}&", _httpUtility.UrlEncode(SitefinityClaimTypes.UserName), name);
            sb.AppendFormat("{0}={1}&", _httpUtility.UrlEncode(SitefinityClaimTypes.Domain), result.SiteFinityRelyingParty.Domain);
            sb.AppendFormat("{0}={1}&", _httpUtility.UrlEncode(SitefinityClaimTypes.AuthentificationMethod), _httpUtility.UrlEncode("http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/password"));
            sb.AppendFormat("{0}={1}&", _httpUtility.UrlEncode(SitefinityClaimTypes.AuthentificationInstant), DateTime.UtcNow);
           



            //double lifeTimeInSeconds = 3600;
            var loginDateClaim = claims.FirstOrDefault(x => x.Type == SitefinityClaimTypes.LastLoginDate);
            DateTime issueDate = DateTime.UtcNow;
            if (loginDateClaim != null)
            {
                if (!DateTime.TryParseExact(loginDateClaim.Value, "u", null, DateTimeStyles.None, out issueDate))
                {
                    issueDate = DateTime.UtcNow;
                }
            }

            sb
                .AppendFormat("TokenId={0}&", _httpUtility.UrlEncode(Guid.NewGuid().ToString()))
                .AppendFormat("Issuer={0}&", _httpUtility.UrlEncode(result.Issuer))
                .AppendFormat("Audience={0}&", _httpUtility.UrlEncode(result.Realm))
                .AppendFormat("ExpiresOn={0:0}", (issueDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + SimpleWebTokenParser.tokenLifeTime);
            //.AppendFormat("IssueDate={0:0}", (issueDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

            var unsignedToken = sb.ToString();

            var hmac = new HMACSHA256(key);
            var sig = hmac.ComputeHash(Encoding.ASCII.GetBytes(unsignedToken));

            string signedToken = String.Format("{0}&HMACSHA256={1}",
                unsignedToken,
                _httpUtility.UrlEncode(Convert.ToBase64String(sig)));

            return  _simpleWebTokenParser.GetToken(signedToken);
        }

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private void WrapSWT(NameValueCollection collection, SimpleWebToken token, bool deflate)
        {
            var rawToken = token.RawToken;
            if (deflate)
            {
                var zipped = ZipStr(rawToken);
                rawToken = Convert.ToBase64String(zipped);
                collection["wrap_deflated"] = "true";
            }
            collection["wrap_access_token"] = _httpUtility.UrlEncode(rawToken);
            var seconds = Convert.ToInt32((token.ValidTo - token.ValidFrom).TotalSeconds);
            collection["wrap_access_token_expires_in"] = seconds.ToString();
        }

        private byte[] ZipStr(String str)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream gzip = new DeflateStream(output, CompressionMode.Compress))
                {
                    using (StreamWriter writer = new StreamWriter(gzip, Encoding.UTF8))
                    {
                        writer.Write(str);
                    }
                }

                return output.ToArray();
            }
        }
        private string ToQueryString(NameValueCollection collection, bool startWithQuestionMark = true)
        {
            if (collection == null || !collection.HasKeys())
                return String.Empty;

            var sb = new StringBuilder();
            if (startWithQuestionMark)
                sb.Append("?");

            var j = 0;
            var keys = collection.Keys;
            foreach (string key in keys)
            {
                var i = 0;
                var values = collection.GetValues(key);
                foreach (var value in values)
                {
                    sb.Append(key)
                        .Append("=")
                        .Append(value);

                    if (++i < values.Length)
                        sb.Append("&");
                }
                if (++j < keys.Count)
                    sb.Append("&");
            }
            return sb.ToString();
        }

        public NameValueCollection ParseQueryString(string s)
        {
            NameValueCollection nvc = new NameValueCollection();

            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }

            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] singlePair = Regex.Split(vp, "=");
                if (singlePair.Length == 2)
                {
                    nvc.Add(singlePair[0], singlePair[1]);
                }
                else
                { // only one key with no value specified in query string
                    nvc.Add(singlePair[0], string.Empty);
                }
            }

            return nvc;
        }

    }
}