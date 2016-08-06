using System.Threading.Tasks;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
using Newtonsoft.Json;
using Xunit;

namespace HttpProblemDetails.Nancy.Tests
{
    public class HttpProblemDetailsShould
    {
        private readonly Browser _browser;

        public HttpProblemDetailsShould()
        {
            _browser = new Browser(with =>
            {
                with.RequestStartup((c, p, ctx) => HttpProblemDetails.Enable(p, c.Resolve<IResponseNegotiator>()));
                with.Module<PaymentModule>();
            });

        }

        [Fact]
        public async Task ReturnSuccess()
        {
            var result = await _browser.Get("/payment/67890", with => 
            {
                with.HttpRequest();
                with.Accept(new MediaRange("application/json"));
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("OK", result.Body.AsString());
        }

        [Fact]
        public async Task ReturnInsufficientCash()
        {
            var result = await _browser.Get("/payment/12345", with => 
            {
                with.HttpRequest();
                with.Accept(new MediaRange("application/json"));
            });

            Assert.Equal("application/problem+json", result.ContentType);
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);

            dynamic problemDetail = JsonConvert.DeserializeObject(result.Body.AsString());
            Assert.NotNull(problemDetail);
            Assert.Equal("https://example.com/probs/out-of-credit", problemDetail.type.ToString());
            Assert.Equal("You do not have enough credit.", problemDetail.title.ToString());
            Assert.Equal("403", problemDetail.status.ToString());
            Assert.Equal("Your current balance is 30, but that costs 50.", problemDetail.detail.ToString());
            Assert.Equal("/account/12345/msgs/abc", problemDetail.instance.ToString());
        }
    }
}
