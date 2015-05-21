using System;
using Autofac;
using Autofac.Integration.WebApi;
using IdentityServer.SiteFinity.ResponseHandling;
using IdentityServer.SiteFinity.Utilities;
using IdentityServer.SiteFinity.Validation;
using Microsoft.Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.SiteFinity.Configuration.Hosting
{
    internal static class AutofacConfig
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public static IContainer Configure(SiteFinityPluginOptions options)
        {
            if (options == null) throw new ArgumentNullException("options");

            var factory = options.Factory;
            factory.Validate();

            var builder = new ContainerBuilder();

            // mandatory from factory
            builder.Register(factory.UserService);
            builder.Register(factory.SiteFinityRelyingPartyService);

            // Utilities
            builder.RegisterType<HttpUtility>().AsSelf();

            //Token parser
            builder.RegisterType<SimpleWebTokenParser>().AsSelf();

            // validators
            builder.RegisterType<SignInValidator>().AsSelf();

            // processors
            builder.RegisterType<SignInResponseGenerator>().AsSelf();
            //builder.RegisterType<MetadataResponseGenerator>().AsSelf();

            // general services
            //builder.RegisterType<CookieMiddlewareTrackingCookieService>().As<ITrackingCookieService>();
            builder.RegisterInstance(options).AsSelf();
            builder.RegisterInstance(options.IdentityServerOptions).AsSelf();

            // load core controller
            builder.RegisterApiControllers(typeof(SiteFinityController).Assembly);

            builder.Register(resolver => new OwinEnvironmentService(resolver.Resolve<IOwinContext>().Environment));

            // register additional dependencies from identity server
            foreach (var registration in options.IdentityServerOptions.Factory.Registrations)
            {
                builder.Register(registration);
            }

            // add any additional dependencies from hosting application
            foreach (var registration in factory.Registrations)
            {
                builder.Register(registration, registration.Name);
            }

            return builder.Build();
        }

        private static void Register(this ContainerBuilder builder, Registration registration, string name = null)
        {
            if (registration.Instance != null)
            {
                var reg = builder.Register(ctx => registration.Instance).SingleInstance();
                if (name != null)
                {
                    reg.Named(name, registration.DependencyType);
                }
                else
                {
                    reg.As(registration.DependencyType);
                }
            }
            else if (registration.Type != null)
            {
                var reg = builder.RegisterType(registration.Type);
                if (name != null)
                {
                    reg.Named(name, registration.DependencyType);
                }
                else
                {
                    reg.As(registration.DependencyType);
                }
            }
            else if (registration.Factory != null)
            {
                var reg = builder.Register(ctx => registration.Factory(new AutofacDependencyResolver(ctx)));
                if (name != null)
                {
                    reg.Named(name, registration.DependencyType);
                }
                else
                {
                    reg.As(registration.DependencyType);
                }
            }
            else
            {
                var message = "No type or factory found on registration " + registration.GetType().FullName;
                Logger.Error(message);
                throw new InvalidOperationException(message);
            }
        }
    }
}