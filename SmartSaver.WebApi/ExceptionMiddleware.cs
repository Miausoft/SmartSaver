using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using SmartSaver.Domain.CustomExceptions;

namespace SmartSaver.WebApi
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (InvalidModelException e)
            {
                await HandleGlobalExceptionAsync(httpContext, e);
            }
        }

        private Task HandleGlobalExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return httpContext.Response.WriteAsync(new GlobalErrorDetails()
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = "Something went wrong!"
            }.ToString());
        }
    }

    class GlobalErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
