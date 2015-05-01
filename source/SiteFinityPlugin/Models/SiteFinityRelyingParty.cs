namespace IdentityServer.SiteFinity.Models
{
    /// <summary>
    /// Models a Sitefinity Site
    /// </summary>
    public class SiteFinityRelyingParty
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SiteFinityRelyingParty"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        public string Realm { get; set; }

        /// <summary>
        /// Gets or sets the Sitefinity ShareSecret
        /// </summary>
        /// <value>
        /// The SiteFinity Secret Key
        /// </value>
        public string Key { get; set; }

        public SiteFinityRelyingParty()
        {
            Enabled = true;
        }
    }
}
