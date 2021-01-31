using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
using System.Linq;

namespace SmartSaver.Domain.ActionFilters
{
    public class RequiresAccountAttribute : ActionFilterAttribute
    {
        public RequiresAccountAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var accountRepo = filterContext
                .HttpContext.RequestServices
                .GetService(typeof(IRepository<Account>)) as Repository<Account>;

            if (!accountRepo.GetAll().Any(a => a.UserId.ToString().Equals(filterContext.HttpContext.User.Identity.Name)))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Account",
                            action = "Complete",
                            returnUrl = filterContext.HttpContext.Request.Path
                        })
                    );
            }
        }
    }
}
