using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Linq;

namespace SmartSaver.Domain.CustomAttributes
{
    public class RequiresAccountAttribute : ActionFilterAttribute
    {
        public RequiresAccountAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var accountRepo = filterContext
                .HttpContext.RequestServices
                .GetService(typeof(IRepository<Account>)) as Repository<Account>;

            var account = accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(filterContext.HttpContext.User.Identity.Name) &&
                                a.Name.Equals(filterContext.HttpContext.Request.Query["name"]))
                .FirstOrDefault();

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
            else if (account == null || !account.UserId.ToString().Equals(filterContext.HttpContext.User.Identity.Name))
            {
                filterContext.HttpContext.Response.StatusCode = 404;
            }
        }
    }
}
