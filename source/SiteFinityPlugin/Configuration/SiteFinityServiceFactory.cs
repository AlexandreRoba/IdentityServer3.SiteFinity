using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.SiteFinity.Configuration
{
    public class SiteFinityServiceFactory
    {
        /// <summary>
        /// Gets or sets the user service.
        /// </summary>
        /// <value>
        /// The user service.
        /// </value>
        public Registration<IUserService> UserService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteFinityServiceFactory"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public SiteFinityServiceFactory(IdentityServerServiceFactory factory)
        {
            UserService = factory.UserService;
        }
    }
}
