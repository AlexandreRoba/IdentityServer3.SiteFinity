using System.Security.Claims;
using System.Security.Principal;
using IdentityServer.SiteFinity.Models;
using IdentityServer.SiteFinity.Services;

namespace IdentityServer.SiteFinity.Validation
{
    public class SignInValidationResult
    {
        public string Realm { get; set; }
        public bool IsSignInRequired { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
        public SiteFinityRelyingParty SiteFinityRelyingParty { get; set; }
        public SignInRequestMessage SignInRequestMessage { get; set; }
        public ClaimsPrincipal Subject { get; set; }
        public string ReplyUrl { get; set; }
        public string Issuer { get; set; }
    }
}