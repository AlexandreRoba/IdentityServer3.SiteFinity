using IdentityServer.SiteFinity.Validation;
using Thinktecture.IdentityServer.Core.Extensions;

namespace IdentityServer.SiteFinity.Models
{

    internal class SignInValidationLog
    {
        public SignInValidationLog(SignInValidationResult result)
        {
            if (result.SiteFinityRelyingParty != null)
            {
                Realm = result.SiteFinityRelyingParty.Realm;
                RelyingPartyName = result.SiteFinityRelyingParty.Name;
            }

            if (Subject != null)
            {
                Subject = result.Subject.GetSubjectId();
            }

            ReplyUrl = result.ReplyUrl;
        }

        public string Realm { get; set; }
        public string RelyingPartyName { get; set; }
        public string ReplyUrl { get; set; }
        public string Subject { get; set; }
    }

}