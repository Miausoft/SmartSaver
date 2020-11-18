using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.MVC.Models;

namespace SmartSaver.MVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(ErrorViewModel model, int statusCode)
        {
            switch(statusCode)
            {
                case 404:
                    model.RequestId = "404";
                    break;
            }
            return View(nameof(Error), model);
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error(ErrorViewModel model)
        {
            //the part where we will deal with the exception and log them to our db:
            /*var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            exceptionDetails.Path;
            exceptionDetails.Error.Message;
            exceptionDetails.Error.StackTrace;*/

            return View(nameof(Error), model);
        }
    }
}
