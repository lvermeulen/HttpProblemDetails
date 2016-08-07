using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace HttpProblemDetails.WebApi
{
    public class HttpProblemDetailsExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private string GetContentTypeStringForContext(HttpActionExecutedContext context)
        {
            var accept = context.Request.Headers.Accept
                .Select(x => x.MediaType)
                .ToArray();

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

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var ex = HttpProblemDetailException.FromException(actionExecutedContext.Exception);
            if (ex == null)
            {
                return;
            }

            // get formatter from content negotiator
            var configuration = actionExecutedContext.ActionContext.ControllerContext.Configuration;
            var contentNegotiator = configuration.Services.GetContentNegotiator();
            var connegResult = contentNegotiator.Negotiate(typeof(IHttpProblemDetail), actionExecutedContext.Request, configuration.Formatters);
            var formatter = connegResult.Formatter;

            // return object content
            actionExecutedContext.Response = new HttpResponseMessage((HttpStatusCode)ex.ProblemDetail.Status)
            {
                Content = new ObjectContent(typeof(IHttpProblemDetail), ex.ProblemDetail, formatter, GetContentTypeStringForContext(actionExecutedContext))
            };
        }
    }
}
