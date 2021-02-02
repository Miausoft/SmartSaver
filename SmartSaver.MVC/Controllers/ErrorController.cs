using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.CustomExceptions;

namespace SmartSaver.MVC.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger _logger;

        public ErrorController(ILogger logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(ErrorViewModel model, int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 400:
                    model.RequestId = "400";
                    break;
                case 401:
                    model.RequestId = "401";
                    break;
                case 404:
                    model.RequestId = "404";
                    break;
            }

            _logger.Warning($"{statusCode} Error Occured. Path = {statusCodeResult?.OriginalPath}" + 
                $"and QueryString = {statusCodeResult?.OriginalQueryString}");

            return View(nameof(Error), model);
        }

        [Route("Error")]
        public IActionResult Error(ErrorViewModel model)
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            model.RequestId = exceptionDetails?.Error is HttpStatusException httpException ? ((int)httpException.StatusCode).ToString() : "500";

            _logger.Error($"The path {exceptionDetails?.Path} threw an exception " +
                 $"{exceptionDetails?.Error}");

            return View(nameof(Error), model);
        }
    }
}
