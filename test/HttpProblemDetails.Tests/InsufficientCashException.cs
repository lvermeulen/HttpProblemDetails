namespace HttpProblemDetails.Tests
{
    public class InsufficientCashException : HttpProblemDetailException<InsufficientCashProblem>
    {
        public InsufficientCashException(IHttpProblemDetail problemDetail) 
            : base(problemDetail)
        { }
    }
}
