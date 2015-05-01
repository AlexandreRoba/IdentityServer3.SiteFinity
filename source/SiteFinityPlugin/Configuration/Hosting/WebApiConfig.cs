using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

namespace IdentityServer.SiteFinity.Configuration.Hosting
{
    internal static class WebApiConfig
    {
        public static HttpConfiguration Configure(SiteFinityPluginOptions options)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.SuppressDefaultHostAuthentication();

            //config.MessageHandlers.Insert(0, new KatanaDependencyResolver());
            //config.Services.Add(typeof(IExceptionLogger), new LogProviderExceptionLogger());
            //config.Services.Replace(typeof(IHttpControllerTypeResolver), new HttpControllerTypeResolver());

            //config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            if (options.IdentityServerOptions.LoggingOptions.EnableWebApiDiagnostics)
            {
                //var liblog = new TraceSource("LibLog");
                //liblog.Switch.Level = SourceLevels.All;
                //liblog.Listeners.Add(new LibLogTraceListener());

                //var diag = config.EnableSystemDiagnosticsTracing();
                //diag.IsVerbose = options.IdentityServerOptions.LoggingOptions.WebApiDiagnosticsIsVerbose;
                //diag.TraceSource = liblog;
            }

            if (options.IdentityServerOptions.LoggingOptions.EnableHttpLogging)
            {
                //config.MessageHandlers.Add(new RequestResponseLogger());
            }

            return config;
        }


        //private class HttpControllerTypeResolver : IHttpControllerTypeResolver
        //{
        //    public ICollection<Type> GetControllerTypes(IAssembliesResolver _)
        //    {
        //        var httpControllerType = typeof(IHttpController);
        //        return typeof(WebApiConfig)
        //            .Assembly
        //            .GetTypes()
        //            .Where(t => t.IsClass && !t.IsAbstract && httpControllerType.IsAssignableFrom(t))
        //            .ToList();
        //    }
        //}
    }
}
