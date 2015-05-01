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
                    Name = "SiteFinityWebApp01",
                    Realm = "http://localhost:60876",
                    Enabled = true,
                    Key = "6CFC44D3F6245339321DD4D7E681306C21E544130ADBB3287EE8878F78B86D6C"
                }
                
            };
        }
    }
}