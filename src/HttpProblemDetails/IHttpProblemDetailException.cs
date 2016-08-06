namespace HttpProblemDetails
{
    public interface IHttpProblemDetailException
    {
        int StatusCode { get; }

        IHttpProblemDetail ProblemDetail { get; }
    }
}
