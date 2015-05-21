using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityServer.SiteFinity
{
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

        public override DateTime ValidFrom
        {
            get
            {
                return _validFrom;
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                return _validTo;
            }
        }

        public string Issuer { get; private set; }

        public string Audience { get; private set; }

        public IList<Claim> Claims { get; private set; }

        public string RawToken { get; private set; }
    }

}