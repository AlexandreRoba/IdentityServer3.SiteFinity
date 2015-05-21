using System.Net;
using System.Text.RegularExpressions;

namespace IdentityServer.SiteFinity.Utilities
{
    public class HttpUtility
    {
        public string UrlEncode(string input)
        {
            var encodedInput = WebUtility.UrlEncode(input);
            encodedInput = Regex.Replace(encodedInput, "(%[0-9A-F]{2})", c => c.Value.ToLowerInvariant());
            return encodedInput;
        }

        public string UrlDecode(string input)
        {
            return WebUtility.UrlDecode(input);
        }

 
    }
}