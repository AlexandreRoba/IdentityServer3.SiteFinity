using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using IdentityServer.SiteFinity.Configuration.Hosting;
using IdentityServer.SiteFinity.ResponseHandling;
using IdentityServer.SiteFinity.Services;
using IdentityServer.SiteFinity.Validation;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Extensions;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.SiteFinity
{
    [NoCache]
    [HostAuthentication(Constants.PrimaryAuthenticationType)]
    [RoutePrefix("")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SiteFinityController : ApiController
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly SignInValidator _validator;
        private readonly SignInResponseGenerator _signInResponseGenerator;

        public SiteFinityController(SignInValidator validator, SignInResponseGenerator signInResponseGenerator)
        {
            _validator = validator;
            _signInResponseGenerator = signInResponseGenerator;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get(string realm = "", string tokenType = "", string redirect_uri = "", bool deflate = false, bool sign_out = false)
        {
            var message = new SignInRequestMessage(realm, tokenType, redirect_uri, deflate, sign_out);

            var result = await _validator.ValidateAsync(Request.RequestUri.AbsoluteUri, message, User as ClaimsPrincipal);

            if (result.IsSignInRequired)
            {
                Logger.Info("Redirect to login page");
                return RedirectToLogin();
            }

            if (result.IsError)
            {
                Logger.Error(result.Error);
                return BadRequest(result.Error);
            }

            var responseMessage = await _signInResponseGenerator.GenerateResponseAsync(message, result);
            if (responseMessage.IsRedirect)
            {
                return Redirect(responseMessage.Url) as IHttpActionResult;
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, responseMessage.Content);
            return ResponseMessage(response);


        }










        private IHttpActionResult RedirectToLogin()
        {
            Uri publicRequestUri = GetPublicRequestUri();

            var message = new SignInMessage();
            message.ReturnUrl = publicRequestUri.ToString();

            var env = Request.GetOwinEnvironment();
            var url = env.CreateSignInRequest(message);

            return Redirect(url);
        }

        private Uri GetPublicRequestUri()
        {
            string identityServerHost = Request.GetOwinContext()
                                               .Environment
                                               .GetIdentityServerHost();

            string pathAndQuery = Request.RequestUri.PathAndQuery;
            string requestUriString = identityServerHost + pathAndQuery;
            var requestUri = new Uri(requestUriString);

            return requestUri;
        }
    }
}
