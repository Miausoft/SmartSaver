using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartSaver.MVC.Models;

namespace SmartSaver.MVC.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(ErrorViewModel model, int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch(statusCode)
            {
                case 404:
                    _logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath}" +
                        $"and QueryString = {statusCodeResult.OriginalQueryString}");
                    model.RequestId = "404";
                    break;
            }
            return View(nameof(Error), model);
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error(ErrorViewModel model)
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

           _logger.LogError($"The path {exceptionDetails.Path} threw an exception " +
                $"{exceptionDetails.Error}");

            return View(nameof(Error), model);
        }
    }
}
