using System;

namespace HttpProblemDetails.Tests
{
    public class InsufficientCashProblem : IHttpProblemDetail
    {
        public Uri Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public Uri Instance { get; set; }
    }
}
