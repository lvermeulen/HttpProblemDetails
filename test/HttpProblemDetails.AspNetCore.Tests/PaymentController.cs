using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using HttpProblemDetails.Tests;

namespace HttpProblemDetails.AspNetCore.Tests
{
    public class PaymentController : Controller
    {
        [Route("/payment/{account}")]
        public string GetPayment(string account)
        {
            if (account == "12345")
            {
                var problemDetail = new InsufficientCashProblem
                {
                    Type = new Uri("https://example.com/probs/out-of-credit"),
                    Title = "You do not have enough credit.",
                    Status = (int)HttpStatusCode.Forbidden,
                    Detail = "Your current balance is 30, but that costs 50.",
                    Instance = new Uri("/account/12345/msgs/abc", UriKind.Relative)
                };
                throw new InsufficientCashException(problemDetail);
            }

            return "OK";
        }
    }
}
