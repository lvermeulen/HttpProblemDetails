using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace HttpProblemDetails.AspNetCore.Tests
{
    public abstract class HttpProblemDetailsTestBase<TStartup>
        where TStartup : class
    {
        [Fact]
        public async Task ReturnSuccess()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<TStartup>());
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "http://host:6789/payment/67890");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            Assert.Equal("OK", await responseMessage.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ReturnInsufficientCash()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<ExceptionFilterStartup>());
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "http://host:6789/payment/12345");
            var client = server.CreateClient();
            var responseMessage = await client.SendAsync(requestMessage);

            Assert.Equal("application/problem+json", responseMessage.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);

            dynamic problemDetail = JsonConvert.DeserializeObject(await responseMessage.Content.ReadAsStringAsync());
            Assert.NotNull(problemDetail);
            Assert.Equal("https://example.com/probs/out-of-credit", problemDetail.Type.ToString());
            Assert.Equal("You do not have enough credit.", problemDetail.Title.ToString());
            Assert.Equal("403", problemDetail.Status.ToString());
            Assert.Equal("Your current balance is 30, but that costs 50.", problemDetail.Detail.ToString());
            Assert.Equal("/account/12345/msgs/abc", problemDetail.Instance.ToString());
        }
    }
}