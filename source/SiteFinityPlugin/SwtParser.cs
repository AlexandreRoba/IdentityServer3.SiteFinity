using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace IdentityServer.SiteFinity
{
    public class SwtParser
    {
        public const string IssuerLabel = "Issuer";
        public const string ExpiresLabel = "ExpiresOn";
        //public const string IssueLabel = "IssueDate";
        public const string AudienceLabel = "Audience";
        public const string TokenIdLabel = "TokenId";
        public const string TokenPrefix = "WRAP access_token";

        private readonly IList<KeyValuePair<string, string>> keyValueCollection;
        private readonly DateTime validFrom;
        internal const int tokenLifeTime = 3600;

        public SwtParser(string urlDecodedSWT)
        {
            this.validFrom = DateTime.UtcNow;
            keyValueCollection = Parse(urlDecodedSWT);
        }

        public static string EncryptionLabel
        {
            get
            {
                return "HMACSHA1";
            }
        }

        public List<Claim> Claims
        {
            get
            {
                return
                    keyValueCollection.Where(e =>
                        e.Key != TokenIdLabel &&
                        e.Key != IssuerLabel &&
                        e.Key != ExpiresLabel &&
                        e.Key != AudienceLabel &&
                        e.Key != EncryptionLabel &&
                        e.Key != "HMACSHA256").Select(kv =>
                            new Claim(kv.Key, kv.Value)).ToList();
            }
        }

        public string TokenId { get { return keyValueCollection.First(p => p.Key == TokenIdLabel).Value; } }

        public string Issuer { get { return keyValueCollection.First(p => p.Key == IssuerLabel).Value; } }

        public string Audience { get { return keyValueCollection.First(p => p.Key == AudienceLabel).Value; } }

        public DateTime ExpiresOn
        {
            get
            {
                int expiresOn = Convert.ToInt32(keyValueCollection.First(p => p.Key == ExpiresLabel).Value);
                var epoc = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                return epoc.AddSeconds(expiresOn);

            }
        }

        public DateTime ValidFrom
        {
            get
            {
                return this.ExpiresOn.AddSeconds(-tokenLifeTime);
                //var key = keyValueCollection.FirstOrDefault(p => p.Key == IssueLabel);
                //if (!string.IsNullOrEmpty(key.Value))
                //{
                //    int issuedOn = Convert.ToInt32(key.Value);
                //    var epoc = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //    return epoc.AddSeconds(issuedOn);
                //}
                //return this.validFrom;
            }
        }

        public static IList<KeyValuePair<string, string>> Parse(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException();
            }

            return
                token
                .Split('&')
                .Aggregate(
                new List<KeyValuePair<string, string>>(),
                (dict, rawNameValue) =>
                {
                    if (rawNameValue == string.Empty)
                    {
                        return dict;
                    }

                    string[] nameValue = rawNameValue.Split('=');

                    if (nameValue.Length != 2)
                    {
                        throw new ArgumentException("Invalid formEncodedstring - contains a name/value pair missing an = character", nameValue.Length > 0 ? nameValue[0] : "");
                    }

                    dict.Add(new KeyValuePair<string, string>(WebUtility.UrlDecode(nameValue[0]), WebUtility.UrlDecode(nameValue[1])));
                    return dict;
                });
        }

        public static string ExtractAndDecodeAccessToken(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) ||
                !authorizationHeader.StartsWith(TokenPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            authorizationHeader = authorizationHeader.Remove(0, TokenPrefix.Length).TrimStart(' ');
            if (authorizationHeader[0] != '=')
            {
                return null;
            }

            var accessToken = authorizationHeader.TrimStart('=', ' ').Trim('"');
            return accessToken;
        }
    }
}
