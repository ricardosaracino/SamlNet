using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace SamlNet
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_PostAuthorizeRequest()
        {
           // HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        /* void WSFederationAuthenticationModule_SessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e)
         {
             var handler = (SessionSecurityTokenHandler)FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)];
             var duration = handler.TokenLifetime;
 
             var token = e.SessionToken;
             e.SessionToken =
                 new SessionSecurityToken(
                     token.ClaimsPrincipal,
                     token.Context,
                     token.ValidFrom,
                     token.ValidFrom.Add(duration))
                 {
                     IsPersistent = token.IsPersistent,
                     IsReferenceMode = token.IsReferenceMode
                 };
         }*/


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            FederatedAuthentication.SessionAuthenticationModule.SessionSecurityTokenCreated +=
                new EventHandler<SessionSecurityTokenCreatedEventArgs>(
                    SessionAuthenticationModule_SessionSecurityTokenCreated);
            FederatedAuthentication.SessionAuthenticationModule.SessionSecurityTokenReceived +=
                new EventHandler<SessionSecurityTokenReceivedEventArgs>(
                    SessionAuthenticationModule_SessionSecurityTokenReceived);
            FederatedAuthentication.SessionAuthenticationModule.SigningOut +=
                new EventHandler<SigningOutEventArgs>(SessionAuthenticationModule_SigningOut);
            FederatedAuthentication.SessionAuthenticationModule.SignedOut +=
                new EventHandler(SessionAuthenticationModule_SignedOut);
            FederatedAuthentication.SessionAuthenticationModule.SignOutError +=
                new EventHandler<ErrorEventArgs>(SessionAuthenticationModule_SignOutError);
        }

        void SessionAuthenticationModule_SignOutError(object sender, ErrorEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Handling SignOutError event");
        }

        void SessionAuthenticationModule_SignedOut(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Handling SignedOut event");
        }

        void SessionAuthenticationModule_SigningOut(object sender, SigningOutEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Handling SigningOut event");
        }

        void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender,
            SessionSecurityTokenReceivedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Handling SessionSecurityTokenReceived event");


            // https://brockallen.com/2013/02/17/sliding-sessions-in-wif-with-the-session-authentication-module-sam-and-thinktecture-identitymodel/
            SessionAuthenticationModule sam = FederatedAuthentication.SessionAuthenticationModule;

            var handler =
                (SessionSecurityTokenHandler) FederatedAuthentication.FederationConfiguration.IdentityConfiguration
                    .SecurityTokenHandlers[typeof(SessionSecurityToken)];

            Console.WriteLine("Session Time {0} {1} {2} {3}", handler.TokenLifetime, DateTime.UtcNow,
                e.SessionToken.ValidFrom, e.SessionToken.ValidTo);

            /*var token = e.SessionToken;
            
            e.SessionToken =
                new SessionSecurityToken(
                    token.ClaimsPrincipal,
                    token.Context,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddMinutes(handler.TokenLifetime.Minutes))
                {
                    IsPersistent = token.IsPersistent,
                    IsReferenceMode = token.IsReferenceMode
                };
            
            e.ReissueCookie = true;*/
        }

        void SessionAuthenticationModule_SessionSecurityTokenCreated(object sender,
            SessionSecurityTokenCreatedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Handling SessionSecurityTokenCreated event");
            //Store session on the server-side token cache instead writing the whole token to the cookie.
            //It may improve throughput but introduces server affinity that may affect scalability
            FederatedAuthentication.SessionAuthenticationModule.IsReferenceMode = true;
        }
    }
}