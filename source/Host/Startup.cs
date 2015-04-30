using Host.Config;
using IdentityServer.SiteFinity.Configuration;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;

namespace Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());

            app.Map("/core", coreApp =>
            {
                var factory = InMemoryFactory.Create(
                    users: Users.Get(),
                    clients: Clients.Get(),
                    scopes: Scopes.Get());

                var options = new IdentityServerOptions
                {
                    SiteName = "IdentityServer3 with SiteFinity",

                    SigningCertificate = Certificate.Get(),
                    Factory = factory,
                    PluginConfiguration = ConfigurePlugins,
                };

                coreApp.UseIdentityServer(options);
            });
        }

        private void ConfigurePlugins(IAppBuilder pluginApp, IdentityServerOptions options)
        {
            var siteFinityOptions = new SiteFinityPluginOptions(options);

            //// data sources for in-memory services
            //wsFedOptions.Factory.Register(new Registration<IEnumerable<RelyingParty>>(RelyingParties.Get()));
            //wsFedOptions.Factory.RelyingPartyService = new Registration<IRelyingPartyService>(typeof(InMemoryRelyingPartyService));

            pluginApp.UseSiteFinityPlugin(siteFinityOptions);
        }
    }
}