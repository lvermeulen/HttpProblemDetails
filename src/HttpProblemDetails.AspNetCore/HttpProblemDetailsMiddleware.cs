using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;

namespace HttpProblemDetails.AspNetCore
{
    public class HttpProblemDetailsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ObjectResultExecutor _objectResultExecutor;

        public HttpProblemDetailsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, ObjectResultExecutor objectResultExecutor)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            if (objectResultExecutor == null)
            {
                throw new ArgumentNullException(nameof(objectResultExecutor));
            }

            _next = next;
            _logger = loggerFactory.CreateLogger<HttpProblemDetailsMiddleware>();
            _objectResultExecutor = objectResultExecutor;
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
                    _logger.LogWarning("Middleware not executing because response has already started");
                    throw;
                }

                //var ex = HttpProblemDetailException.FromException(ex);
                //if (ex == null)
                //{
                //    return;
                //}

                context.HandleProblemDetailsException(ex);
                //context.Response.Clear();
                //await _objectResultExecutor.ExecuteAsync(new ActionContext { HttpContext = context }, new ObjectResult(ex.ProblemDetail)
                //{
                //    StatusCode = ex.ProblemDetail.Status,
                //    //ContentTypes = new MediaTypeCollection { new MediaTypeHeaderValue(GetContentTypeStringForContext(context.HttpContext)) },
                //    DeclaredType = typeof(IHttpProblemDetail)
                //});

                //context.Result = new ObjectResult(ex.ProblemDetail)
                //{
                //    StatusCode = ex.ProblemDetail.Status,
                //    //ContentTypes = new MediaTypeCollection { new MediaTypeHeaderValue(GetContentTypeStringForContext(context.HttpContext)) },
                //    DeclaredType = typeof(IHttpProblemDetail)
                //};
            }
        }

        //public async Task Invoke(HttpContext context)
        //{
        //    try
        //    {
        //        await _next.Invoke(context);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (context.Response.HasStarted)
        //        {
        //            _logger.LogWarning("Middleware not executing because response has already started");
        //            throw;
        //        }

        //        //var ex = HttpProblemDetailException.FromException(ex);
        //        //if (ex == null)
        //        //{
        //        //    return;
        //        //}

        //        context.HandleProblemDetailsException(ex);
        //        //context.Response.Clear();
        //        //await _objectResultExecutor.ExecuteAsync(new ActionContext { HttpContext = context }, new ObjectResult(ex.ProblemDetail)
        //        //{
        //        //    StatusCode = ex.ProblemDetail.Status,
        //        //    //ContentTypes = new MediaTypeCollection { new MediaTypeHeaderValue(GetContentTypeStringForContext(context.HttpContext)) },
        //        //    DeclaredType = typeof(IHttpProblemDetail)
        //        //});

        //        //context.Result = new ObjectResult(ex.ProblemDetail)
        //        //{
        //        //    StatusCode = ex.ProblemDetail.Status,
        //        //    //ContentTypes = new MediaTypeCollection { new MediaTypeHeaderValue(GetContentTypeStringForContext(context.HttpContext)) },
        //        //    DeclaredType = typeof(IHttpProblemDetail)
        //        //};
        //    }
        //}
    }
}
