using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SamlNet.Models;

namespace SamlNet.Handlers
{
    public class UserHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // if NOT cached or expired 
            // read from database
            // user in cache with nameid

            // set request.Properties from cache

            var currentUser = HttpContext.Current.User as ClaimsPrincipal;

            var claims = currentUser?.Claims;
            var nameIdentifierClaim = claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var nameId = nameIdentifierClaim?.Value;
            
            request.Properties["currentUser"] = new CurrentUser()
            {
                Id = Guid.NewGuid()
            };

            return base.SendAsync(request, cancellationToken).ContinueWith((task) =>
            {
                var a = request.Properties["currentUser"];
                
                // if modified add back to cache

                return task.Result;
            });
        }
    }
}