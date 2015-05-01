using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.SiteFinity.Models;

namespace IdentityServer.SiteFinity.Services
{
    /// <summary>
    /// In-memory service for relying party configuration
    /// </summary>
    public class InMemoryRelyingPartyService : ISiteFinityRelyingPartyService
    {
        readonly IEnumerable<SiteFinityRelyingParty> _rps;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRelyingPartyService"/> class.
        /// </summary>
        /// <param name="rps">The RPS.</param>
        public InMemoryRelyingPartyService(IEnumerable<SiteFinityRelyingParty> rps)
        {
            _rps = rps;
        }

        /// <summary>
        /// Retrieves a relying party by realm.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns>
        /// The relying party
        /// </returns>
        public Task<SiteFinityRelyingParty> GetByRealmAsync(string realm)
        {
            return Task.FromResult(_rps.FirstOrDefault(rp => rp.Realm == realm && rp.Enabled));
        }
    }
}