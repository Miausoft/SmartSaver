using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace SmartSaver.Domain.CustomAttributes
{
    public class AnonymousOnlyAttribute : ActionFilterAttribute
    {
        public AnonymousOnlyAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
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
