namespace HttpProblemDetails
{
    public interface IHttpProblemDetailException
    {
        IHttpProblemDetail ProblemDetail { get; }
    }
}
