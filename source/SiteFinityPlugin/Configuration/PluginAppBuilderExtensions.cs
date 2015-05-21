using System;
using IdentityServer.SiteFinity.Configuration.Hosting;
using Microsoft.Owin.Infrastructure;
using Owin;

namespace IdentityServer.SiteFinity.Configuration
{
    /// <summary>
    /// Extension methods for IAppBuilder to configure the WS-Federation plugin
    /// </summary>
    public static class PluginAppBuilderExtensions
    {
        /// <summary>
        /// Add the WS-Federation plugin to the IdentityServer pipeline.
        /// </summary>
        /// <param name="app">The appBuilder.</param>
        /// <param name="options">The options.</param>
        /// <returns>The appBuilder</returns>
        /// <exception cref="System.ArgumentNullException">options</exception>
        public static IAppBuilder UseSiteFinityPlugin(this IAppBuilder app, SiteFinityPluginOptions options)
        {
            if (options == null) throw new ArgumentNullException("options");
            options.Validate();

            //options.IdentityServerOptions.ProtocolLogoutUrls.Add(options.LogoutUrl);

            app.Map(options.MapPath, sitefinityApp =>
            {
                //sitefinityApp.UseCookieAuthentication(new CookieAuthenticationOptions
                //{
                //    AuthenticationType = WsFederationPluginOptions.CookieName,
                //    AuthenticationMode = AuthenticationMode.Passive,
                //    CookieName = options.IdentityServerOptions.AuthenticationOptions.CookieOptions.Prefix + WsFederationPluginOptions.CookieName,
                //});

                sitefinityApp.Use<AutofacContainerMiddleware>(AutofacConfig.Configure(options));
                SignatureConversions.AddConversions(app);
                sitefinityApp.UseWebApi(WebApiConfig.Configure(options));
            });

            return app;
        }
    }
}
