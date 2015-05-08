namespace IdentityServer.SiteFinity.Services
{
    public class SignInRequestMessage
    {
        public string Realm { get; private set; }
        public string TokenType { get; private set; }
        public string RedirectUri { get; private set; }
        public bool Deflate { get; private set; }
        public bool SignOut { get; private set; }

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