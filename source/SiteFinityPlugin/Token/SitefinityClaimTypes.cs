namespace IdentityServer.SiteFinity.Token
{
    /// <summary>
    /// A list of predifined Claims
    /// </summary>
    public static class SitefinityClaimTypes
    {
        internal const string UserName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        internal const string TokenId = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/tokenid";
        internal const string UserId = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/userid";
        internal const string Domain = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/domain";
        internal const string Role = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/role";
        internal const string IssueDate = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/issuedate";
        internal const string LastLoginDate = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/lastlogindate";
        internal const string Adjusted = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/adjusted";
        internal const string StsType = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/ststype";

        internal const string AuthentificationMethod = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod";
        internal const string AuthentificationInstant = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant";
    }
}
