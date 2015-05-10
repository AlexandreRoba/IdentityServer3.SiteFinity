using System.Net.Http;

namespace IdentityServer.SiteFinity.ResponseHandling
{
    public class SignInResponseMessage
    {
        public bool IsRedirect { get; set; }
        public StringContent Content { get; set; }

        public string Url { get; set; }

    }
}
