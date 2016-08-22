using System;

namespace HttpProblemDetails
{
    public interface IHttpProblemDetail
    {
        string Type { get; }
        string Title { get; }
        int Status { get; }
        string Detail { get; }
        string Instance { get; }
    }
}
