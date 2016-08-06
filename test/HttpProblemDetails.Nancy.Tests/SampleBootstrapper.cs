using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.TinyIoc;

namespace HttpProblemDetails.Nancy.Tests
{
    public class SampleBootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            HttpProblemDetails.Enable(pipelines, container.Resolve<IResponseNegotiator>());
        }
    }
}
