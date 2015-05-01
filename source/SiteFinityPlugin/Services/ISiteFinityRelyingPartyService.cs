using System.Threading.Tasks;
using IdentityServer.SiteFinity.Models;

namespace IdentityServer.SiteFinity.Services
{
    /// <summary>
    /// Implements retrieval of SiteFinity relying party configuration
    /// </summary>
    public interface ISiteFinityRelyingPartyService
    {
        /// <summary>
        /// Retrieves a sitefinity relying party by realm.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns>The sitefinity relying party</returns>
        Task<SiteFinityRelyingParty> GetByRealmAsync(string realm);
    }
}
