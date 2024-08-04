using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace AHHA.API.Filters
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        private const string Token = "Token";
        //public override void OnActionExecuting(HttpActionContext filterContext)
        //{
        //    // Get API key provider
        //    var provider = filterContext.ControllerContext.Configuration.DependencyResolver.GetService(typeof(ITokenServices)) as ITokenServices;

        //    if (filterContext.Request.Headers.Contains(Token))
        //    {
        //        var tokenValue = filterContext.Request.Headers.GetValues(Token).First();

        //        // Validate Token
        //        if (provider != null && !provider.ValidateToken(tokenValue))
        //        {
        //            var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        //            {
        //                ReasonPhrase = "Invalid Request"
        //            };
        //            filterContext.Response = responseMessage;
        //        }
        //    }
        //    else
        //    {
        //        filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //    }
        //    base.OnActionExecuting(filterContext);
        //}
    }
}
