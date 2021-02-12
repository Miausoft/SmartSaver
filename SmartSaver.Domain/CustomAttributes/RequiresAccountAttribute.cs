using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
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

            var userId = filterContext.HttpContext.User.Identity.Name;
            var accountName = (filterContext.HttpContext.GetRouteValue("name") ?? string.Empty).ToString();

            if (!accountRepo.SearchFor(a => a.UserId.ToString().Equals(userId)).Any())
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Account",
                            action = "Complete"
                        })
                    );
            }
            else if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accountName))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Account"
                        })
                    );
            }
            else if (!accountRepo.SearchFor(a => a.UserId.ToString().Equals(userId) && a.Name.Equals(accountName)).Any())
            {
                filterContext.Result = new NotFoundResult();
            }
        }
    }
}
