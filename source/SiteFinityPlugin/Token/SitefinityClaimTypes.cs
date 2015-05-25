namespace IdentityServer.SiteFinity.Token
{
    public static class SitefinityClaimTypes
    {
        public const string UserName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public const string TokenId = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/tokenid";
        public const string UserId = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/userid";
        public const string Domain = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/domain";
        public const string Role = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/role";
        public const string IssueDate = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/issuedate";
        public const string LastLoginDate = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/lastlogindate";
        public const string Adjusted = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/adjusted";
        public const string StsType = "http://schemas.sitefinity.com/ws/2011/06/identity/claims/ststype";
    
        public const string AuthentificationMethod = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod";
        public const string AuthentificationInstant = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant";
    }
}
