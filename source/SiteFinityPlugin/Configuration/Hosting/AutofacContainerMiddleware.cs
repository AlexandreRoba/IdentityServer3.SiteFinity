using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin;

namespace IdentityServer.SiteFinity.Configuration.Hosting
{
    internal class AutofacContainerMiddleware
    {
        readonly private Func<IDictionary<string, object>, Task> _next;
        readonly private IContainer _container;

        public AutofacContainerMiddleware(Func<IDictionary<string, object>, Task> next, IContainer container)
        {
            _next = next;
            _container = container;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);

            // this creates a per-request, disposable scope
            using (var scope = _container.BeginLifetimeScope(b =>
            {
                // this makes owin context resolvable in the scope
                b.RegisterInstance(context).As<IOwinContext>();
            }))
            {
                // this makes scope available for downstream frameworks
                context.Set("idsrv:SiteFinityAutofacScope", scope);
                await _next(env);
            }
        }
    }
}