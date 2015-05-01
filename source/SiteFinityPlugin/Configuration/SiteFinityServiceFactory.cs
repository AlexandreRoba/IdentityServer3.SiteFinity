using System;
using System.Collections.Generic;
using IdentityServer.SiteFinity.Services;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.SiteFinity.Configuration
{
    public class SiteFinityServiceFactory
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the user service.
        /// </summary>
        /// <value>
        /// The user service.
        /// </value>
        public Registration<IUserService> UserService { get; set; }

        // keep list of any additional dependencies the 
        // hosting application might need. these will be
        // added to the DI container
        readonly List<Registration> _registrations = new List<Registration>();

        /// <summary>
        /// Gets the a list of additional dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public IEnumerable<Registration> Registrations
        {
            get { return _registrations; }
        }

        /// <summary>
        /// Adds a registration to the dependency list
        /// </summary>
        /// <typeparam name="T">Type of the dependency</typeparam>
        /// <param name="registration">The registration.</param>
        public void Register<T>(Registration<T> registration)
            where T : class
        {
            _registrations.Add(registration);
        }


        /// <summary>
        /// Gets or sets the SiteFinity relying party service.
        /// </summary>
        /// <value>
        /// The relying party service.
        /// </value>
        public Registration<ISiteFinityRelyingPartyService> SiteFinityRelyingPartyService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteFinityServiceFactory"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public SiteFinityServiceFactory(IdentityServerServiceFactory factory)
        {
            UserService = factory.UserService;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            if (UserService == null) LogAndStop("UserService not configured");
            if (SiteFinityRelyingPartyService == null) LogAndStop("SiteFinity RelyingPartyService not configured");
        }

        private void LogAndStop(string message)
        {
            Logger.Error(message);
            throw new InvalidOperationException(message);
        }
    }
}
