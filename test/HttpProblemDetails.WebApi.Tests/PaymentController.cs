using System;
using System.Net;
using System.Web.Http;
using HttpProblemDetails.Tests;

namespace HttpProblemDetails.WebApi.Tests
{
    public class PaymentController : ApiController
    {
        [Route("/payment/{account}")]
        public string GetPayment(string account)
        {
            if (account == "12345")
            {
                var problemDetail = new InsufficientCashProblem
                {
                    Type = new Uri("https://example.com/probs/out-of-credit").ToString(),
                    Title = "You do not have enough credit.",
                    Status = (int)HttpStatusCode.Forbidden,
                    Detail = "Your current balance is 30, but that costs 50.",
                    Instance = new Uri("/account/12345/msgs/abc", UriKind.Relative).ToString()
                };
                throw new InsufficientCashException(problemDetail);
            }

            return "OK";
        }
    }
}
