//using System;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using System.Web.Http.Hosting;
//using System.Web.Http.Routing;
using HttpProblemDetails.Tests;
using Xunit;

namespace HttpProblemDetails.WebApi.Tests
{
    public class HttpProblemDetailsExceptionFilterAttributeShould
    {
        //private HttpActionExecutedContext CreateActionExecutedContext(Exception exception, Action<HttpRequestMessage> requestMessageFactory = null)
        //{
        //    var configuration = new HttpConfiguration();

        //    var httpRouteData = new HttpRouteData(new HttpRoute());
        //    var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
        //    request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

        //    requestMessageFactory?.Invoke(request);

        //    var controllerContext = new HttpControllerContext(configuration, httpRouteData, request);
        //    var context = new HttpActionContext(controllerContext, new ReflectedHttpActionDescriptor());
        //    var executedContext = new HttpActionExecutedContext(context, exception);
        //    return executedContext;
        //}

        /*
            if (account == "12345")
            {
                var problemDetail = new InsufficientCashProblem
                {
                    Type = new Uri("https://example.com/probs/out-of-credit"),
                    Title = "You do not have enough credit.",
                    Status = 403,
                    Detail = "Your current balance is 30, but that costs 50.",
                    Instance = new Uri("/account/12345/msgs/abc", UriKind.Relative)
                };
                throw new InsufficientCashException(problemDetail.Status, problemDetail);
            }

            return "OK";
        */

        [Fact]
        public void ReturnSuccess()
        {
            var controller = new PaymentController();
            var result = controller.GetPayment("67890");

            Assert.Equal("OK", result);
        }

        [Fact]
        public void ReturnInsufficientCash()
        {
            var controller = new PaymentController();
            Assert.Throws<InsufficientCashException>(() =>
            {
                controller.GetPayment("12345");
            });
        }

        //[Fact]
        //public void TestContentNegotiation(string contentType, string expectedResponseContentType)
        //{
        //    var problemDetail = new InsufficientCashProblem
        //    {

        //    };
        //    var exception = new InsufficientCashException((int)HttpStatusCode.Forbidden, problemDetail);
        //    var filter = new HttpProblemDetailsExceptionFilterAttribute();
        //    var actionExecutedContext = CreateActionExecutedContext(exception, 
        //        request => request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType)));
        //    filter.OnException(actionExecutedContext);

        //    Assert.Equal(HttpStatusCode.BadRequest, actionExecutedContext.Response.StatusCode);
        //    Assert.Equal(expectedResponseContentType, actionExecutedContext.Response.Content.Headers.ContentType.MediaType);
        //}
    }
}
