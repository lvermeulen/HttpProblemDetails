![Icon](/assets/noun_560850_cc.png?raw=true)
# HttpProblemDetails [![Build status](https://ci.appveyor.com/api/projects/status/63u4fo3onib9xlng?svg=true)](https://ci.appveyor.com/project/lvermeulen/httpproblemdetails) [![license](https://img.shields.io/badge/license-MIT-blue.svg?maxAge=2592000)](https://github.com/lvermeulen/httpproblemdetails/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/vpre/httpproblemdetails.aspnetcore.svg?maxAge=2592000)](https://www.nuget.org/packages/httpproblemdetails.aspnetcore/) [![Coverage Status](https://coveralls.io/repos/github/lvermeulen/HttpProblemDetails/badge.svg?branch=master)](https://coveralls.io/github/lvermeulen/HttpProblemDetails?branch=master) [![Dependency Status](https://dependencyci.com/github/lvermeulen/HttpProblemDetails/badge)](https://dependencyci.com/github/lvermeulen/HttpProblemDetails) [![Join the chat at https://gitter.im/lvermeulen/Equalizer](https://badges.gitter.im/lvermeulen/httpproblemdetails.svg)](https://gitter.im/lvermeulen/httpproblemdetails?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) ![](https://img.shields.io/badge/.net-4.5.2-yellowgreen.svg) ![](https://img.shields.io/badge/netstandard-1.6-yellowgreen.svg)
A library to render problem details as specified by RFC 7807 at [https://tools.ietf.org/rfc/rfc7807.txt](https://tools.ietf.org/rfc/rfc7807.txt), with implementations for WebApi, AspNetCore and Nancy.

## Features:
* Web Api ExceptionFilterAttribute
* AspNetCore ExceptionFilter
* AspNetCore Middleware
* Nancy pipelines extension

## Usage:

Implement interface IHttpProblemDetailException in your exceptions, which is an exception with an IHttpProblemDetail. The IHttpProblemDetail carries the fields required by the RFC and will be rendered in your HTTP response.

The tests use exception InsufficientCashException with problem detail InsufficientCashProblem following the example in the RFC.

* AspNetCore ExceptionFilter:

Step 1: Register the exception filter in your Startup class
~~~~
    public class ExceptionFilterStartup
    {
        private ILoggerFactory _loggerFactory;

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new HttpProblemDetailsExceptionFilter(_loggerFactory));
            });
        }
    }
~~~~

Step 2: Throw an exception with a problem detail in your controller
~~~~
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
                    Status = 403,
                    Detail = "Your current balance is 30, but that costs 50.",
                    Instance = new Uri("/account/12345/msgs/abc", UriKind.Relative)
                };
                throw new InsufficientCashException(problemDetail.Status, problemDetail);
            }

            return "OK";
        }
    }
~~~~

* AspNetCore Middleware:

Step 1: Use the middleware in your Startup class
~~~~
    public class MiddlewareStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHttpProblemDetails();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
    }
~~~~

Step 2: Throw an exception with a problem detail in your controller
~~~~
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
                    Status = 403,
                    Detail = "Your current balance is 30, but that costs 50.",
                    Instance = new Uri("/account/12345/msgs/abc", UriKind.Relative)
                };
                throw new InsufficientCashException(problemDetail.Status, problemDetail);
            }

            return "OK";
        }
    }
~~~~

* Nancy pipelines extension:

Step 1: Enable the extension
~~~~
    public class SampleBootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            HttpProblemDetails.Enable(pipelines, container.Resolve<IResponseNegotiator>());
        }
    }
~~~~

Step 2: Throw an exception with a problem detail in your module
~~~~
    public class PaymentModule : NancyModule
    {
        public PaymentModule()
        {
            Get("/payment/{account}", async args =>
            {
                if (args.account == "12345")
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

                return await Response
                    .AsText("OK")
                    .WithStatusCode(HttpStatusCode.OK);
            });
        }
    }
~~~~

* Web Api ExceptionFilterAttribute

Step 1: Add the exception filter to your configuration, or apply the attribute to a controller or method

Configuration:
~~~~
	GlobalConfiguration.Configuration.Filters.Add(
	    new HttpProblemDetailsExceptionFilterAttribute());
~~~~

Controller or method:
~~~~
	[HttpProblemDetailsExceptionFilter]
    public class PaymentController : ApiController
    {
        [Route("/payment/{account}")]
		[HttpProblemDetailsExceptionFilter]
        public string GetPayment(string account)
        {
			...
		}
	}
~~~~

Step 2: Throw an exception with a problem detail in your controller
~~~~
    public class PaymentController : ApiController
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
                    Status = 403,
                    Detail = "Your current balance is 30, but that costs 50.",
                    Instance = new Uri("/account/12345/msgs/abc", UriKind.Relative)
                };
                throw new InsufficientCashException(problemDetail.Status, problemDetail);
            }

            return "OK";
        }
    }
~~~~

## Thanks
* [Laptop Alert](https://thenounproject.com/term/laptop-alert/560850/) icon by [arejoenah](https://thenounproject.com/arejoenah/) from [The Noun Project](https://thenounproject.com)
