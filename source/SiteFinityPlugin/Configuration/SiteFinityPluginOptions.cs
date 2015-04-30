using System;
using Thinktecture.IdentityServer.Core.Configuration;

namespace IdentityServer.SiteFinity.Configuration
{
    public class SiteFinityPluginOptions
    {
        /// <summary>
        /// Gets or sets the identity server options.
        /// </summary>
        /// <value>
        /// The identity server options.
        /// </value>
        public IdentityServerOptions IdentityServerOptions { get; set; }

        /// <summary>
        /// Gets or sets the map path.
        /// </summary>
        /// <value>
        /// The map path.
        /// </value>
        public string MapPath { get; set; }

        /// <summary>
        /// Gets or sets the WS-Federation service factory.
        /// </summary>
        /// <value>
        /// The factory.
        /// </value>
        public SiteFinityServiceFactory Factory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteFinityPluginOptions"/> class.
        /// </summary>
        public SiteFinityPluginOptions()
        {
            MapPath = "/sitefinity";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteFinityPluginOptions"/> class.
        /// Assigns the IdentityServerOptions and the Factory from the IdentityServerOptions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">options</exception>
        public SiteFinityPluginOptions(IdentityServerOptions options) : this()
        {
            if (options == null) throw new ArgumentNullException("options");
            
            IdentityServerOptions = options;
            this.Factory = new SiteFinityServiceFactory(options.Factory);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Factory not configured
        /// or
        /// DataProtector not configured
        /// or
        /// Options not configured
        /// </exception>
        public void Validate()
        {
            if (Factory == null)
            {
                throw new ArgumentNullException("Factory not configured");
            }
            
            if (IdentityServerOptions == null)
            {
                throw new ArgumentNullException("Options not configured");
            }
        }
    }
}
