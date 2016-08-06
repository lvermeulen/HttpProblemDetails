using System;
using Nancy;

namespace HttpProblemDetails.Nancy
{
    public abstract class HttpProblemDetailException<T> : Exception, IHttpProblemDetailException 
        where T : IHttpProblemDetail
    {
        public HttpStatusCode StatusCode { get; }
        public IHttpProblemDetail ProblemDetail { get; }
        public override string Message => ProblemDetail.Detail;

        protected HttpProblemDetailException(HttpStatusCode statusCode, IHttpProblemDetail problemDetail)
        {
            StatusCode = statusCode;
            ProblemDetail = problemDetail;
        }
    }
}
