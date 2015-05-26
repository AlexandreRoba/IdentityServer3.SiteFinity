using System.Net;
using System.Text.RegularExpressions;

namespace IdentityServer.SiteFinity.Utilities
{
    /// <summary>
    /// Class that handles the Url Encoding
    /// </summary>
    /// <remarks>
    /// This class is here to support the difference in the encoding betwen the System.Web urlencode/UrlDecode and the Katana/Owin supported encoding.
    /// The difference relies in upper and lower caps. 
    /// </remarks>
    public class HttpUtility
    {
        /// <summary>
        /// Url encode a value
        /// </summary>
        /// <param name="input">The value to be encoded</param>
        /// <returns>The url encoded value</returns>
        public string UrlEncode(string input)
        {
            var encodedInput = WebUtility.UrlEncode(input);
            encodedInput = Regex.Replace(encodedInput, "(%[0-9A-F]{2})", c => c.Value.ToLowerInvariant());
            return encodedInput;
        }
        /// <summary>
        /// URlDecode a value
        /// </summary>
        /// <param name="input">The value to be decoded</param>
        /// <returns>The decoded value</returns>
        public string UrlDecode(string input)
        {
            return WebUtility.UrlDecode(input);
        }

 
    }
}