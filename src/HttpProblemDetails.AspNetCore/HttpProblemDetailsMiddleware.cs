using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HttpProblemDetails.AspNetCore
{
    public class HttpProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpProblemDetailsMiddleware(RequestDelegate next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.HandleProblemDetailsException(ex);
            }
        }
    }
}
