using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityServer.SiteFinity.Configuration.Hosting;
using IdentityServer.SiteFinity.Services;
using IdentityServer.SiteFinity.Validation;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Extensions;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.SiteFinity
{
    [NoCache]
    [HostAuthentication(Constants.PrimaryAuthenticationType)]
    [RoutePrefix("")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SiteFinityController : ApiController
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly SignInValidator _validator;

        public SiteFinityController(SignInValidator validator)
        {
            _validator = validator;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get(string realm = "", string tokenType = "", string redirect_uri = "", bool deflate = false, bool sign_out = false)
        {
            var message = new SignInRequestMessage(realm, tokenType, redirect_uri, deflate, sign_out);

            var result = await _validator.ValidateAsync(message, User as ClaimsPrincipal);

            if (result.IsSignInRequired)
            {
                Logger.Info("Redirect to login page");
                return await RedirectToLogin();
            }

            //TODO: Need to lear the Identity Service Token service and replace this custom code. 
            //I'll start by using custom code first.
            //var reply = result.ReplyUrl;
            
            var principal = new ClaimsPrincipal(User);
            var identity = ClaimsPrincipal.PrimaryIdentitySelector(principal.Identities);

            var issuer = Request.RequestUri.AbsoluteUri;
            var idx = issuer.IndexOf("?");
            if (idx != -1)
                issuer = issuer.Substring(0, idx);

            

            var token = this.CreateToken(identity.Claims, issuer, realm);
            NameValueCollection queryString;
            if (!String.IsNullOrEmpty(reply))
            {
                string path;
                idx = reply.IndexOf('?');
                if (idx != -1)
                {
                    path = reply.Substring(0, idx);
                    queryString = new Uri(reply.Substring(idx + 1)).ParseQueryString();// HttpUtility.ParseQueryString(reply.Substring(idx + 1));
                }
                else
                {
                    path = reply;
                    queryString = new NameValueCollection();
                }
                this.WrapSWT(queryString, token, deflate);
                path = String.Concat(path, ToQueryString(queryString));
                var uri = new Uri(new Uri(realm), path);
                return Redirect(uri.AbsoluteUri);
            }

            queryString = new NameValueCollection();
            this.WrapSWT(queryString, token, deflate);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(ToQueryString(queryString, false), Encoding.UTF8, "application/x-www-form-urlencoded");
            
            return ResponseMessage(response);
        }

       

        private async Task<SimpleWebToken> CreateToken(IEnumerable<Claim> claims, string issuerName, string appliesTo)
        {
            appliesTo = appliesTo.ToLower();

            //var sKey = ConfigurationManager.AppSettings[appliesTo];
            //if (String.IsNullOrEmpty(sKey))
            //{
            //    //check if appliesTo failed to find the key because it's missing a trailing slash, 
            //    //or because it has a trailing slash which shouldn't be there
            //    //and act accordingly (and try again):
            //    if (!appliesTo.EndsWith("/"))
            //        appliesTo += "/";
            //    else
            //        appliesTo = VirtualPathUtility.RemoveTrailingSlash(appliesTo);

            //    sKey = ConfigurationManager.AppSettings[appliesTo];
            //    if (String.IsNullOrEmpty(sKey))
            //        throw new ConfigurationException(String.Format("Missing symmetric key for \"{0}\".", appliesTo));
            //}
            //var key = this.HexToByte(sKey);
            var key = this.HexToByte("HelloWorld");

            var sb = new StringBuilder();
            foreach (var c in claims)
                sb.AppendFormat("{0}={1}&", WebUtility.UrlEncode(c.Type), WebUtility.UrlEncode(c.Value));

            sb.AppendFormat("{0}={1}&", SitefinityClaimTypes.StsType, "wa");

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
                .AppendFormat("TokenId={0}&", WebUtility.UrlEncode(Guid.NewGuid().ToString()))
                .AppendFormat("Issuer={0}&", WebUtility.UrlEncode(issuerName))
                .AppendFormat("Audience={0}&", WebUtility.UrlEncode(appliesTo))
                .AppendFormat("ExpiresOn={0:0}", (issueDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + SwtParser.tokenLifeTime);
            //.AppendFormat("IssueDate={0:0}", (issueDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

            var unsignedToken = sb.ToString();

            var hmac = new HMACSHA256(key);
            var sig = hmac.ComputeHash(Encoding.ASCII.GetBytes(unsignedToken));

            string signedToken = String.Format("{0}&HMACSHA256={1}",
                unsignedToken,
                WebUtility.UrlEncode(Convert.ToBase64String(sig)));

            return new SimpleWebToken(signedToken);
        }

        private void WrapSWT(NameValueCollection collection, SimpleWebToken token, bool deflate)
        {
            var rawToken = token.RawToken;
            if (deflate)
            {
                var zipped = this.ZipStr(rawToken);
                rawToken = Convert.ToBase64String(zipped);
                collection["wrap_deflated"] = "true";
            }
            collection["wrap_access_token"] = WebUtility.UrlEncode(rawToken);
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

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string ToQueryString(NameValueCollection collection, bool startWithQuestionMark = true)
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
        
        private Task<IHttpActionResult> RedirectToLogin()
        {
            Uri publicRequestUri = GetPublicRequestUri();

            var message = new SignInMessage();
            message.ReturnUrl = publicRequestUri.ToString();

            var env = Request.GetOwinEnvironment();
            var url = env.CreateSignInRequest(message);

            return Redirect(url);
        }

        private Uri GetPublicRequestUri()
        {
            string identityServerHost = Request.GetOwinContext()
                                               .Environment
                                               .GetIdentityServerHost();

            string pathAndQuery = Request.RequestUri.PathAndQuery;
            string requestUriString = identityServerHost + pathAndQuery;
            var requestUri = new Uri(requestUriString);

            return requestUri;
        }
    }
}
