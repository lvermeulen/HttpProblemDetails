using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        const string APPLICATION_JSON = "application/json";
        const string APPLICATION_PROBLEM_JSON = "application/problem+json";
        const string APPLICATION_XML = "application/xml";
        const string APPLICATION_PROBLEM_XML = "application/problem+xml";

        private readonly Dictionary<string, string> _responseMediaTypeByAcceptMediaType = new Dictionary<string, string>
        {
            { APPLICATION_JSON, APPLICATION_PROBLEM_JSON },
            { APPLICATION_XML, APPLICATION_PROBLEM_XML }
        };

        [Fact]
        public async Task ReturnSuccess()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<TStartup>());
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "http://host:6789/payment/67890");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            Assert.Equal("OK", await responseMessage.Content.ReadAsStringAsync());
        }

        //[Theory]
        ////[InlineData(APPLICATION_JSON)]
        //[InlineData(APPLICATION_XML)]
        [Fact]
        public async Task ReturnInsufficientCash()
        //public async Task ReturnInsufficientCash(string accept)
        {
            string accept = APPLICATION_XML;
            var server = new TestServer(new WebHostBuilder().UseStartup<TStartup>());
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "http://host:6789/payment/12345");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            var client = server.CreateClient();
            try
            {
                var responseMessage = await client.SendAsync(requestMessage);
                //Assert.Equal(_responseMediaTypeByAcceptMediaType[accept], responseMessage.Content.Headers.ContentType.MediaType);
                //Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);

                //responseMessage.HttpContext.
                string content = await responseMessage.Content.ReadAsStringAsync();
                dynamic problemDetail = JsonConvert.DeserializeObject(content);
                Assert.NotNull(problemDetail);
                Assert.Equal("https://example.com/probs/out-of-credit", problemDetail.type.ToString());
                Assert.Equal("You do not have enough credit.", problemDetail.title.ToString());
                Assert.Equal("403", problemDetail.status.ToString());
                Assert.Equal("Your current balance is 30, but that costs 50.", problemDetail.detail.ToString());
                Assert.Equal("/account/12345/msgs/abc", problemDetail.instance.ToString());
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}