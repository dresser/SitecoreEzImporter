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
            configuration.Routes.MapHttpRoute("ImportApi", "sitecore/api/EzImporter/Import", new
            {
                controller = "Import",
                action = "Import"
            });
            configuration.Routes.MapHttpRoute("ImportSettings", "sitecore/api/EzImporter/DefaultSettings", new
            {
                controller = "Import",
                action = "DefaultSettings"
            });
        }
    }
}