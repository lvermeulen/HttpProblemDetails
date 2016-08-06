using Nancy;

namespace HttpProblemDetails.Nancy
{
    public interface IHttpProblemDetailException
    {
        HttpStatusCode StatusCode { get; }

        IHttpProblemDetail ProblemDetail { get; }
    }
}
