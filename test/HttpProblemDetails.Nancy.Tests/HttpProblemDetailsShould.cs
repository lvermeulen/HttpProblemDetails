using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
using Xunit;
using HttpProblemDetails.Tests;

namespace HttpProblemDetails.Nancy.Tests
{
    public class HttpProblemDetailsShould
    {
        private readonly Browser _browser;

        public HttpProblemDetailsShould()
        {
            _browser = new Browser(with =>
            {
                with.RequestStartup((container, pipelines, context) => HttpProblemDetails.Enable(pipelines, container.Resolve<IResponseNegotiator>()));
                with.Module<PaymentModule>();
            });

        }

        [Fact]
        public async Task ReturnSuccessAsync()
        {
            var result = await _browser.Get("/payment/67890", with => 
            {
                with.HttpRequest();
                with.Accept(new MediaRange("application/json"));
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("OK", result.Body.AsString());
        }

        [Theory]
        [InlineData("application/json", "application/problem+json")]
        [InlineData("application/xml", "application/problem+xml")]
        public async Task ReturnInsufficientCashAsync(string accept, string expectedContentType)
        {
            var assertByAccept = new Dictionary<string, Action<string>>
            {
                { "application/json", DeserializationHelper.AssertFromJson },
                { "application/xml", DeserializationHelper.AssertFromXml }
            };
            var result = await _browser.Get("/payment/12345", with => 
            {
                with.HttpRequest();
                with.Accept(new MediaRange(accept));
            });

            Assert.Equal(expectedContentType, result.ContentType);
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);

            var body = result.Body.AsString();
            assertByAccept[accept](body);
        }
    }
}
