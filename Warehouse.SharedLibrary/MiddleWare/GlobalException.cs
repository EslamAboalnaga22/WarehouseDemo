using Microsoft.AspNetCore.Http;
using Warehouse.SharedLibrary.Logs;
using System.Net;
using System.Text.Json;

namespace Warehouse.SharedLibrary.MiddleWare
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare Variables
            string message = "Sorry, Internal Server Error";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                // check if Exception is Too Many Reqyset // 429 status code
                if(context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    message = "Too Many Requests";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    title = "Warning";

                    await ModifyHeader(context , title, message, statusCode);
                }

                // check if Response is Unauthorized // 401 status code
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    message = "You Are Unauthorized";
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Alert";

                    await ModifyHeader(context, title, message, statusCode);
                }

                // check if Response is Forbidden // 403 status code
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    message = "You Are Forbidden";
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Out Of Access";

                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch(Exception ex)
            {
                // Logs
                LogsException.LogExceptions(ex);

                // Check if Excpetion is Timeout
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    message = "Request Timeout";
                    statusCode = StatusCodes.Status408RequestTimeout;
                    title = "Out Of Time";
                }

                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var problemDetails = new
            {
                Title = title,
                Message = message,
                StatusCode = statusCode
            };

            await context.Response.WriteAsJsonAsync(problemDetails);

            return;
        }
    }
}
