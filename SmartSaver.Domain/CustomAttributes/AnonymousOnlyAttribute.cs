using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace SmartSaver.Domain.CustomAttributes
{
    public class AnonymousOnlyAttribute : ActionFilterAttribute
    {
        public AnonymousOnlyAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Account",
                            action = "Index",
                            returnUrl = filterContext.HttpContext.Request.Path
                        })
                    );
            }
        }
    }
}
