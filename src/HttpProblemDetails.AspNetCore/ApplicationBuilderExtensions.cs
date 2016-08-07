using System;
using Microsoft.AspNetCore.Builder;

namespace HttpProblemDetails.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHttpProblemDetails(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<HttpProblemDetailsMiddleware>();
        }
    }
}
