using System;

namespace HttpProblemDetails.Nancy
{
    public interface IHttpProblemDetail
    {
        Uri Type { get; }
        string Title { get; }
        int Status { get; }
        string Detail { get; }
        Uri Instance { get; }
    }
}
