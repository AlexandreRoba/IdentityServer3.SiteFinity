using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityServer.SiteFinity
{
    public class SimpleWebToken : SecurityToken
    {
        /// <summary>
        /// Creates a new instance of SimpleWebToken and optionally parses it
        /// </summary>
        /// <param name="rawToken">URL decoded SWT</param>
        /// <param name="autoParse">true if parsing is required, otherwise false.</param>
        public SimpleWebToken(string rawToken)
        {
            this.RawToken = rawToken;
            this.EnsureProperties();
        }

        public string RawToken { get; private set; }

        public override string Id
        {
            get
            {
                return this.tokenId;
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
                return this.validFrom;
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                return this.validTo;
            }
        }

        public string Issuer
        {
            get
            {
                return this.issuer;
            }
        }

        public string Audience
        {
            get
            {
                return this.audience;
            }
        }

        public IList<Claim> Claims
        {
            get
            {
                return this.claims;
            }
        }

        void EnsureProperties()
        {
            var parser = new SwtParser(this.RawToken);

            this.issuer = parser.Issuer;
            this.audience = parser.Audience;
            this.validFrom = parser.ValidFrom;
            this.validTo = parser.ExpiresOn;
            this.claims = parser.Claims;
            this.tokenId = parser.TokenId;
        }

        private string tokenId;
        private string issuer;
        private string audience;
        private DateTime validFrom;
        private DateTime validTo;
        private IList<Claim> claims;
    }

}