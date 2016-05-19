using Sitecore.Diagnostics;

namespace EzImporter.Pipelines.ImportItems
{
    public class ValidateArgs : ImportItemsProcessor
    {
        public override void Process(ImportItemsArgs args)
        {
            Log.Info("EzImporter:Validating input...", this);
            var argsValid = true;
            if (args.FileStream == null)
            {
                Log.Error("EzImporter:Input file not found.", this);
                argsValid = false;
            }
            if (!argsValid)
            {
                args.AbortPipeline();
            }
        }
    }
}