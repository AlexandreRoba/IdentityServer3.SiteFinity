using Autofac;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.SiteFinity.Configuration.Hosting
{
    internal class AutofacDependencyResolver : IDependencyResolver
    {
        readonly IComponentContext _ctx;
        public AutofacDependencyResolver(IComponentContext ctx)
        {
            _ctx = ctx;
        }

        public T Resolve<T>(string name)
        {
            if (name != null)
            {
                return _ctx.ResolveNamed<T>(name);
            }

            return _ctx.Resolve<T>();
        }
    }

}