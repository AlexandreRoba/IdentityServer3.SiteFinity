namespace IdentityServer.SiteFinity.Services
{
    /// <summary>
    /// The Sign in Request message
    /// </summary>
    public class SignInRequestMessage
    {
        /// <summary>
        /// The realm of the sign request
        /// </summary>
        public string Realm { get; private set; }
        /// <summary>
        /// The token type of the signin request
        /// </summary>
        public string TokenType { get; private set; }
        /// <summary>
        /// The redirect URI from the sign in request
        /// </summary>
        public string RedirectUri { get; private set; }
        /// <summary>
        /// A flag that idicate if the sign in request is defalted or not
        /// </summary>
        public bool Deflate { get; private set; }
        /// <summary>
        /// A flag that indicate if the signin request is a signout request
        /// </summary>
        public bool SignOut { get; private set; }

        /// <summary>
        /// Value constructor
        /// </summary>
        /// <param name="realm">The realm of the sign request</param>
        /// <param name="tokenType">The token type of the signin request</param>
        /// <param name="redirectUri">The redirect URI from the sign in request</param>
        /// <param name="deflate">A flag that idicate if the sign in request is defalted or not</param>
        /// <param name="signOut">A flag that indicate if the signin request is a signout request</param>
        public SignInRequestMessage(string realm = "", string tokenType = "", string redirectUri = "", bool deflate = false, bool signOut = false)
        {
            Realm = realm;
            TokenType = tokenType;
            RedirectUri = redirectUri;
            Deflate = deflate;
            SignOut = signOut;
        }

        
    }
}