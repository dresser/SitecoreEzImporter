using Sitecore.Pipelines;
using System.Web.Http;

namespace EzImporter.Pipelines
{
    public class RegisterHttpRoutes
    {
        public void Process(PipelineArgs args)
        {
            GlobalConfiguration.Configure(Configure);
        }

        protected void Configure(HttpConfiguration configuration)
        {
            var routes = configuration.Routes;
            routes.MapHttpRoute("ImportApi", "sitecore/api/EzImporter/Import", new
            {
                controller = "Import",
                action = "Import"
            });
        }
    }
}