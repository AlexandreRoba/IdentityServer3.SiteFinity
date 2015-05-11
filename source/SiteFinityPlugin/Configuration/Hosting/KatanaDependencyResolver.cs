using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Hosting;
using Autofac;

namespace IdentityServer.SiteFinity.Configuration.Hosting
{
    internal class KatanaDependencyResolver : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var owin = request.GetOwinContext();
            var scope = owin.Get<ILifetimeScope>("idsrv:SiteFinityAutofacScope");
            if (scope != null)
            {
                request.Properties[HttpPropertyKeys.DependencyScope] = new AutofacScope(scope);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}