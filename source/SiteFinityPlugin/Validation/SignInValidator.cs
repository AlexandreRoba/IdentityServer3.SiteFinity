﻿using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.SiteFinity.Models;
using IdentityServer.SiteFinity.Services;
using Thinktecture.IdentityServer.Core.Logging;

namespace IdentityServer.SiteFinity.Validation
{
    /// <summary>
    /// Class used to validate a sign in request
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SignInValidator
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly ISiteFinityRelyingPartyService _siteFinityRelyingPartyService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="siteFinityRelyingPartyService">The Relying party used for the validation</param>
        public SignInValidator(ISiteFinityRelyingPartyService siteFinityRelyingPartyService)
        {
            _siteFinityRelyingPartyService = siteFinityRelyingPartyService;
        }

        /// <summary>
        /// This method validates the Sign in request
        /// </summary>
        /// <param name="requestAbsoluteUri">the url of the request</param>
        /// <param name="message">the sign in request message</param>
        /// <param name="subject">The sign in subject</param>
        /// <returns>The validation result</returns>
        public async Task<SignInValidationResult> ValidateAsync(string requestAbsoluteUri, SignInRequestMessage message, ClaimsPrincipal subject)
        {
            Logger.Info("Start SiteFinity signin request validation");
            var result = new SignInValidationResult();

            if (!String.IsNullOrWhiteSpace(message.Realm))
            {
                result.Realm = message.Realm;
            }

            if (message.SignOut)
            {
                if (!subject.Identity.IsAuthenticated)
                {
                    LogError("Signout requested for user not signed in.",result);

                    return new SignInValidationResult
                    {
                        IsError = true,
                        Error = "signout_requested_when_not_signedin"
                    };
                }
                return new SignInValidationResult
                {
                    IsSignout = true,
                };
            }

            if (!subject.Identity.IsAuthenticated)
            {
                result.IsSignInRequired = true;
                return result;
            }

            

            var rp = await _siteFinityRelyingPartyService.GetByRealmAsync(message.Realm);

            if (rp == null || rp.Enabled == false)
            {
                LogError("SiteFinity Relying party not found: " + message.Realm, result);

                return new SignInValidationResult
                {
                    IsError = true,
                    Error = "invalid_sitefinity_relying_party"
                };
            }



            if (string.IsNullOrWhiteSpace(message.RedirectUri))
            {
                if (!string.IsNullOrWhiteSpace(rp.ReplyUrl))
                {
                    result.ReplyUrl = rp.ReplyUrl; 
                }
                else
                {
                    LogError("Reply url is defined or provided for : " + message.Realm, result);

                    return new SignInValidationResult
                    {
                        IsError = true,
                        Error = "missing_replyUrl"
                    };
                }
            }
            else
            {
                result.ReplyUrl = message.RedirectUri;
            }

            result.Issuer = GetIssuerFromRequestUri(requestAbsoluteUri);
            result.SiteFinityRelyingParty = rp;
            result.SignInRequestMessage = message;
            result.Subject = subject;

            LogSuccess(result);
            return result;
        }

        private void LogSuccess(SignInValidationResult result)
        {
            var log = LogSerializer.Serialize(new SignInValidationLog(result));
            Logger.InfoFormat("End WS-Federation signin request validation\n{0}", log);
        }

        private void LogError(string message, SignInValidationResult result)
        {
            var log = LogSerializer.Serialize(new SignInValidationLog(result));
            Logger.ErrorFormat("{0}\n{1}", message, log);
        }

        private string GetIssuerFromRequestUri(string absoluteUri)
        {
            var issuer = absoluteUri;
            var idx = issuer.IndexOf("?");
            if (idx != -1)
                issuer = issuer.Substring(0, idx);
            return issuer;
        }
    }
}
