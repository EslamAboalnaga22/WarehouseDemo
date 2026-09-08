using Microsoft.AspNetCore.Http;

namespace Warehouse.SharedLibrary.MiddleWare
{
    public class ListenOnlyToApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract Specific Header From The Request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            // Null means: The request is not coming from the Api Gateway, so we will return 503 service unavailable


            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                await context.Response.WriteAsync("Access Denied: Invalid API Gateway Request");

                return;
            }
            else
            {
                await next(context);
            }

        }
    }
}
