using System.Collections.Generic;
using IdentityServer.SiteFinity.Models;

namespace Host.Config
{
    public class SiteFinityRelyingParties
    {
        public static IEnumerable<SiteFinityRelyingParty> Get()
        {
            return new List<SiteFinityRelyingParty>
            {   
                new SiteFinityRelyingParty
                {
                    Name = "HelloCrowd",
                    Realm = "https://localhost:44303/",
                    Enabled = true,
                    Key = "52ACD69BD85C96F08C74762ED247A4AAFD2174E6B3E7F700630C2DAC5E169D21"
                }
                
            };
        }
    }
}