using System.Security.Claims;
using System.Security.Principal;
using IdentityServer.SiteFinity.Models;
using IdentityServer.SiteFinity.Services;

namespace IdentityServer.SiteFinity.Validation
{
    /// <summary>
    /// The result of the validation
    /// </summary>
    public class SignInValidationResult
    {
        /// <summary>
        /// The sign request realm
        /// </summary>
        public string Realm { get; set; }
        /// <summary>
        /// Flag indicating if the request require signin
        /// </summary>
        public bool IsSignInRequired { get; set; }
        /// <summary>
        /// Flag indicating if the validation failed
        /// </summary>
        public bool IsError { get; set; }
        /// <summary>
        /// The validation error message
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// The relying party that matches the signin request
        /// </summary>
        public SiteFinityRelyingParty SiteFinityRelyingParty { get; set; }
        /// <summary>
        /// The Sign in request message
        /// </summary>
        public SignInRequestMessage SignInRequestMessage { get; set; }
        /// <summary>
        /// The sign in subject claim
        /// </summary>
        public ClaimsPrincipal Subject { get; set; }
        /// <summary>
        /// The reqeusted reply URL
        /// </summary>
        public string ReplyUrl { get; set; }
        /// <summary>
        /// The sign is request issuer
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// A flag that indicate if the request was a sign out request
        /// </summary>
        public bool IsSignout { get; set; }
    }
}