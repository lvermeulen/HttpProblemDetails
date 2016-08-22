using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace HttpProblemDetails.AspNetCore
{
    public class HttpProblemDetailsExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public HttpProblemDetailsExceptionFilter(ILoggerFactory logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger.CreateLogger(nameof(HttpProblemDetailsExceptionFilter));
        }

        public void OnException(ExceptionContext context)
        {
            //context.HttpContext.HandleProblemDetailsException(context.Exception);
            context.HandleProblemDetailsException(context.Exception);
            context.Exception = null;

            _logger.LogError(nameof(HttpProblemDetailsExceptionFilter), context.Exception);
        }
    }
}
