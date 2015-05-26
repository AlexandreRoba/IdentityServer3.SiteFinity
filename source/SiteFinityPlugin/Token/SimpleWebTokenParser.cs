using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer.SiteFinity.Utilities;

namespace IdentityServer.SiteFinity.Token
{
    /// <summary>
    /// The parser for SimpleWebToken
    /// </summary>
    public class SimpleWebTokenParser
    {
        internal const int TokenLifeTime = 3600;
        
        private const string IssuerLabel = "Issuer";
        private const string ExpiresLabel = "ExpiresOn";
        //public const string IssueLabel = "IssueDate";
        private const string AudienceLabel = "Audience";
        private const string TokenIdLabel = "TokenId";
        private const string TokenPrefix = "WRAP access_token";
        
        private readonly HttpUtility _httpUtility;
        private  IList<KeyValuePair<string, string>> _keyValueCollection;
        private  DateTime _validFrom;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpUtility">the utility class that should be used for url encoding and url decoding</param>
        public SimpleWebTokenParser(HttpUtility httpUtility)
        {
            _httpUtility = httpUtility;
        }
        /// <summary>
        /// Gets the encryption used for the token
        /// </summary>
        private static string EncryptionLabel
        {
            get
            {
                return "HMACSHA1";
            }
        }
        
        private List<Claim> Claims
        {
            get
            {
                return
                    _keyValueCollection.Where(e =>
                        e.Key != TokenIdLabel &&
                        e.Key != IssuerLabel &&
                        e.Key != ExpiresLabel &&
                        e.Key != AudienceLabel &&
                        e.Key != EncryptionLabel &&
                        e.Key != "HMACSHA256").Select(kv =>
                            new Claim(kv.Key, kv.Value)).ToList();
            }
        }

        private string TokenId { get { return _keyValueCollection.First(p => p.Key == TokenIdLabel).Value; } }

        private string Issuer { get { return _keyValueCollection.First(p => p.Key == IssuerLabel).Value; } }

        private string Audience { get { return _keyValueCollection.First(p => p.Key == AudienceLabel).Value; } }

        private DateTime ExpiresOn
        {
            get
            {
                int expiresOn = Convert.ToInt32(_keyValueCollection.First(p => p.Key == ExpiresLabel).Value);
                var epoc = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                return epoc.AddSeconds(expiresOn);

            }
        }

        private DateTime ValidFrom
        {
            get
            {
                return this.ExpiresOn.AddSeconds(-TokenLifeTime);
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

        private IList<KeyValuePair<string, string>> Parse(string token)
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

                    dict.Add(new KeyValuePair<string, string>(_httpUtility.UrlDecode(nameValue[0]), _httpUtility.UrlDecode(nameValue[1])));
                    return dict;
                });
        }

        /// <summary>
        /// Extract and decode the access token
        /// </summary>
        /// <param name="authorizationHeader">The authorization header</param>
        /// <returns>The decoded acces token</returns>
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

        /// <summary>
        /// Gets the token from raw version of the token
        /// </summary>
        /// <param name="rawToken">The raw token</param>
        /// <returns>A Simple web token</returns>
        public  SimpleWebToken GetToken(string rawToken)
        {
             _validFrom = DateTime.UtcNow;
             _keyValueCollection = Parse(rawToken);
             return new SimpleWebToken(TokenId, Issuer, Audience, ValidFrom, ExpiresOn, Claims, rawToken);
        }
    }
}