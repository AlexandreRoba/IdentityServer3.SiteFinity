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
                    Key = "C0EB0636ADA66011DDC5D01C25E0520EEB4645F10318F18A164D32556921B7B8",
                    Domain = "Default"
                }
                
            };
        }
    }
}