using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityServer.SiteFinity.Token
{
    /// <summary>
    /// SimpleWeb Token class
    /// </summary>
    public class SimpleWebToken : SecurityToken
    {

        private readonly string _tokenId;
        private readonly DateTime _validFrom;
        private readonly DateTime _validTo;

        /// <summary>
        /// Creates a new instance of SimpleWebToken
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="validFrom"></param>
        /// <param name="validTo"></param>
        /// <param name="claims"></param>
        /// <param name="rawToken"></param>
        public SimpleWebToken(string tokenId, string issuer, string audience, DateTime validFrom, DateTime validTo, IList<Claim> claims,string rawToken)
        {
            _tokenId = tokenId;
            Issuer = issuer;
            Audience = audience;
            _validFrom = validFrom;
            _validTo = validTo;
            Claims = claims;
            RawToken = rawToken;
        }

        /// <summary>
        /// Gets the token id
        /// </summary>
        public override string Id
        {
            get
            {
                return _tokenId;
            }
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return new List<SecurityKey>().AsReadOnly(); }
        }

        /// <summary>
        /// Gets the Valid From date
        /// </summary>
        public override DateTime ValidFrom
        {
            get
            {
                return _validFrom;
            }
        }

        /// <summary>
        /// Gets the Valid to Date
        /// </summary>
        public override DateTime ValidTo
        {
            get
            {
                return _validTo;
            }
        }

        /// <summary>
        /// Gets the Issuer realm
        /// </summary>
        public string Issuer { get; private set; }

        /// <summary>
        /// Gets the audience realm
        /// </summary>
        public string Audience { get; private set; }

        /// <summary>
        /// Gets the list of Claims in the token
        /// </summary>
        public IList<Claim> Claims { get; private set; }

        /// <summary>
        /// Gets the Raw version of the token
        /// </summary>
        public string RawToken { get; private set; }
    }

}