using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Json;

namespace HttpProblem.Nancy
{
    public class HttpProblem
    {
        private readonly static PipelineItem<Func<NancyContext, Exception, dynamic>> s_httpProblemPipelineItem;

        static HttpProblem()
        {
            s_httpProblemPipelineItem = new PipelineItem<Func<NancyContext, Exception, dynamic>>(nameof(HttpProblem), PrepareHttpProblem);
        }

        /// <summary>
        /// Enable HttpProblem support in the application
        /// </summary>
        /// <param name="pipelines">Application pipeline to hook into</param>
        /// <param name="environment">A <see cref="INancyEnvironment"/> instance.</param>
        public static void Enable(IPipelines pipelines, INancyEnvironment environment)
        {
            var httpProblemEnabled = pipelines.AfterRequest.PipelineItems.Any(ctx => ctx.Name == nameof(HttpProblem));

            if (!httpProblemEnabled)
            {
                //encoding = environment.GetValue<JsonConfiguration>().DefaultEncoding;
                pipelines.OnError.AddItemToEndOfPipeline(s_httpProblemPipelineItem);
            }
        }

        /// <summary>
        /// Disable HttpProblem support in the application
        /// </summary>
        /// <param name="pipelines">Application pipeline to hook into</param>
        public static void Disable(IPipelines pipelines)
        {
            pipelines.AfterRequest.RemoveByName(nameof(HttpProblem));
        }

        private static dynamic PrepareHttpProblem(NancyContext context, Exception exception)
        {
            return null;
        }
    }
}
