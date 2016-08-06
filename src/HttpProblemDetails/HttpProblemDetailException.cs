using System;

namespace HttpProblemDetails
{
    public static class HttpProblemDetailException
    {
        public static IHttpProblemDetailException FromException(Exception ex)
        {
            var exception = ex as IHttpProblemDetailException;
            if (exception != null)
            {
                return exception;
            }

            return ex.InnerException != null
                ? FromException(ex.InnerException)
                : null;
        }
    }

    public abstract class HttpProblemDetailException<T> : Exception, IHttpProblemDetailException 
        where T : IHttpProblemDetail
    {
        public int StatusCode { get; }
        public IHttpProblemDetail ProblemDetail { get; }
        public override string Message => ProblemDetail.Detail;

        protected HttpProblemDetailException(int statusCode, IHttpProblemDetail problemDetail)
        {
            StatusCode = statusCode;
            ProblemDetail = problemDetail;
        }
    }
}
