using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
using SamlNet.Models;

namespace SamlNet.Controllers
{
    [Authorize]
    //[Roles("Volunteer")]
    public class AuthController : ApiController
    {
        [HttpGet]
        [ActionName("Claims")]
        public Dictionary<string, string> Claims()
        {
            // This property is specific to ASP.NET.
            //var user = System.Web.HttpContext.Current.User as ClaimsPrincipal;
            //var claims = user?.Claims;

            var identity = (ClaimsIdentity) User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            var myClaims = new Dictionary<string, string>();

            foreach (var claim in claims)
            {
                myClaims.Add(claim.Type, claim.Value);
            }

            return myClaims;
        }

        [HttpGet]
        [ActionName("NameId")]
        public string NameId()
        {
            //var user = System.Web.HttpContext.Current.User as ClaimsPrincipal;
            //var claims = user?.Claims;

            var identity =  User.Identity as ClaimsIdentity;
            var claims = identity?.Claims;
            var idClaim = claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            return idClaim?.Value;
        }

        [HttpGet]
        [ActionName("IsAuthenticated")]
        [AllowAnonymous]
        public bool? IsAuthenticated()
        {
            var user = System.Web.HttpContext.Current.User as ClaimsPrincipal;

            var identity = user?.Identity;

            return identity?.IsAuthenticated;
        }
        
        [HttpGet]
        [ActionName("Sleep")]
        [AllowAnonymous]
        public string Sleep()
        {
            Thread.Sleep(10 * 1000);
            
            return (Request.Properties["currentUser"] as CurrentUser)?.Id.ToString();
        }

        [HttpGet]
        [ActionName("guid")]
        [AllowAnonymous]
        public string GetGuid()
        {
            var currentUser = Request.Properties["currentUser"] as CurrentUser;

            currentUser.Id = Guid.NewGuid();

            // return System.Web.HttpContext.Current?.Session["test"] as string;
            return currentUser.Id.ToString();
        }
    }
}