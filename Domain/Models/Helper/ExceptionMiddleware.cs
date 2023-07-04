using Microsoft.AspNetCore.Http;
using System.Net;

namespace Project.Domain.Models.Helper
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        //private static readonly RaygunClient _raygunClient = new RaygunClient(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("RaygunSettings").GetSection("ApiKey").Value);

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
            catch (Exception ex)
            {
                //await _raygunClient.SendInBackground(ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // ErrorType.UserUnhandledException.message = exception.Message; // this part is for user defined error response

            return context.Response.WriteAsync(exception.Message); // ErrorType.UserUnhandledException.ToString() // user defined
        }
    }
}
