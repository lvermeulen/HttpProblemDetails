using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HttpProblemDetails.AspNetCore
{
    public static class HttpContextExtensions
    {
        private static string GetContentTypeStringForContext(HttpContext context)
        {
            var accept = context.Request.Headers["Accept"];

            if (accept.Any(x => x == "application/json"))
            {
                return "application/problem+json";
            }

            if (accept.Any(x => x == "application/xml"))
            {
                return "application/problem+xml";
            }

            return accept.FirstOrDefault() ?? "application/problem+json";
        }

        public static void HandleProblemDetailsException(this HttpContext context, Exception exception)
        {
            var ex = HttpProblemDetailException.FromException(exception);
            if (ex == null)
            {
                return;
            }

            string json = JsonConvert.SerializeObject(ex.ProblemDetail);
            context.Response.ContentType = GetContentTypeStringForContext(context);
            context.Response.StatusCode = ex.ProblemDetail.Status;
            var data = Encoding.UTF8.GetBytes(json);
            context.Response.ContentLength = data.Length;
            context.Response.Body.WriteAsync(data, 0, data.Length)
                .Wait();
        }
    }
}
