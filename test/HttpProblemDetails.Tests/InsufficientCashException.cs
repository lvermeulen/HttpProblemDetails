namespace HttpProblemDetails.Tests
{
    public class InsufficientCashException : HttpProblemDetailException<InsufficientCashProblem>
    {
        public InsufficientCashException(int statusCode, IHttpProblemDetail problemDetail) 
            : base(statusCode, problemDetail)
        { }
    }
}
