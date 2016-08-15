using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;
using HttpProblemDetails.Tests;
using Newtonsoft.Json;
using Xunit;

namespace HttpProblemDetails.WebApi.Tests
{
    public class HttpProblemDetailsExceptionFilterAttributeShould
    {
        private HttpActionExecutedContext CreateActionExecutedContext(string url, Exception exception, Action<HttpRequestMessage> requestMessageFactory = null)
        {
            var configuration = new HttpConfiguration();
            configuration.Formatters.XmlFormatter.UseXmlSerializer = true;

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, configuration);
            var requestContext = new HttpRequestContext();
            request.SetRequestContext(requestContext);

            requestMessageFactory?.Invoke(request);

            var controllerDescriptor = new HttpControllerDescriptor(configuration, nameof(PaymentController), typeof(PaymentController));
            var controllerContext = new HttpControllerContext(requestContext, request, controllerDescriptor, new PaymentController());
            controllerContext.Configuration = configuration;
            var actionContext = new HttpActionContext(controllerContext, new ReflectedHttpActionDescriptor(controllerDescriptor, typeof(PaymentController).GetMethod(nameof(PaymentController.GetPayment))));

            var executedContext = new HttpActionExecutedContext(actionContext, exception);
            if (exception == null)
            {
                executedContext.Response = new HttpResponseMessage(HttpStatusCode.OK);
                executedContext.Response.Content = new StringContent("OK");
                executedContext.Response.Content.Headers.ContentType = request.Headers.Accept.FirstOrDefault();
            }

            return executedContext;
        }

        [Theory]
        [InlineData("application/json", "application/json")]
        [InlineData("application/xml", "application/xml")]
        public void ReturnSuccess(string contentType, string expectedResponseContentType)
        {
            var filter = new HttpProblemDetailsExceptionFilterAttribute();
            var actionExecutedContext = CreateActionExecutedContext("http://host:1234/payment/67890", null,
                request => request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType)));
            filter.OnException(actionExecutedContext);

            Assert.Equal(HttpStatusCode.OK, actionExecutedContext.Response.StatusCode);
            Assert.Equal("OK", actionExecutedContext.Response.Content.ReadAsStringAsync().Result);
            Assert.Equal(expectedResponseContentType, actionExecutedContext.Response.Content.Headers.ContentType.MediaType);
        }

        [Theory]
        [InlineData("application/json", "application/problem+json")]
        [InlineData("application/xml", "application/problem+xml")]
        public void ReturnInsufficientCash(string contentType, string expectedResponseContentType)
        {
            var exception = new InsufficientCashException(new InsufficientCashProblem
            {
                Type = new Uri("https://example.com/probs/out-of-credit"),
                Title = "You do not have enough credit.",
                Status = (int)HttpStatusCode.Forbidden,
                Detail = "Your current balance is 30, but that costs 50.",
                Instance = new Uri("/account/12345/msgs/abc", UriKind.Relative)
            });
            var filter = new HttpProblemDetailsExceptionFilterAttribute();
            var actionExecutedContext = CreateActionExecutedContext("http://host:1234/payment/12345", exception,
                request => request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType)));
            filter.OnException(actionExecutedContext);

            Assert.Equal(HttpStatusCode.Forbidden, actionExecutedContext.Response.StatusCode);
            Assert.Equal(expectedResponseContentType, actionExecutedContext.Response.Content.Headers.ContentType.MediaType);

            string body = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
            dynamic problemDetail = JsonConvert.DeserializeObject(body);
            Assert.NotNull(problemDetail);
            Assert.Equal("https://example.com/probs/out-of-credit", problemDetail.Type.ToString());
            Assert.Equal("You do not have enough credit.", problemDetail.Title.ToString());
            Assert.Equal("403", problemDetail.Status.ToString());
            Assert.Equal("Your current balance is 30, but that costs 50.", problemDetail.Detail.ToString());
            Assert.Equal("/account/12345/msgs/abc", problemDetail.Instance.ToString());
        }
    }
}
