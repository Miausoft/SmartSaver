using System.Web.Mvc;
using SmartSaver.Domain.CustomExceptions;

namespace SmartSaver.Domain.CustomAttributes
{
    public class CheckForInvalidModelAttribute : ActionFilterAttribute
    {
        public CheckForInvalidModelAttribute() {}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
        }
    }
}
