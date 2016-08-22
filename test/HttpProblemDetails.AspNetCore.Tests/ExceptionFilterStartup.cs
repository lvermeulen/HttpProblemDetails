using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace HttpProblemDetails.AspNetCore.Tests
{
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
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = false;
                options.InputFormatters.Add(new XmlSerializerInputFormatter());
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                var f1 = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                f1?.SupportedMediaTypes.Add(
                    new MediaTypeHeaderValue("application/problem+json")
                );

                var f2 = options.OutputFormatters.OfType<XmlSerializerOutputFormatter>().FirstOrDefault();
                f2?.SupportedMediaTypes.Add(
                    new MediaTypeHeaderValue("application/problem+xml")
                );

                var f3 = options.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>().FirstOrDefault();
                f3?.SupportedMediaTypes.Add(
                    new MediaTypeHeaderValue("application/problem+xml")
                );

                options.Filters.Add(new HttpProblemDetailsExceptionFilter(_loggerFactory));
            });
        }
    }
}
