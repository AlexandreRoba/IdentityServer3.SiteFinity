using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Logging;

namespace IdentityServer.SiteFinity
{
    [HostAuthentication(Constants.PrimaryAuthenticationType)]
    [RoutePrefix("")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SiteFinityController : ApiController
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            Logger.Info("Start SiteFInity Request");
            Logger.DebugFormat("AbsoluteUri: [{0}]", Request.RequestUri.AbsoluteUri);
            return null;
        }
    }
}
