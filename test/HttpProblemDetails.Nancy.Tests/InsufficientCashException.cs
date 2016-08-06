using Nancy;

namespace HttpProblemDetails.Nancy.Tests
{
    public class InsufficientCashException : HttpProblemDetailException<InsufficientCashProblem>
    {
        public InsufficientCashException(HttpStatusCode statusCode, IHttpProblemDetail problemDetail) 
            : base((int)statusCode, problemDetail)
        { }
    }
}
