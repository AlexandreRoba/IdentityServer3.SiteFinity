using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.SiteFinity.Services;
using Thinktecture.IdentityServer.Core.Logging;

namespace IdentityServer.SiteFinity.Validation
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SignInValidator
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly ISiteFinityRelyingPartyService _siteFinityRelyingPartyService;

        public SignInValidator(ISiteFinityRelyingPartyService siteFinityRelyingPartyService)
        {
            _siteFinityRelyingPartyService = siteFinityRelyingPartyService;
        }
    }
}
