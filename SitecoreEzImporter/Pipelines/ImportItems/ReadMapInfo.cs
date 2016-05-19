using Sitecore.Diagnostics;

namespace EzImporter.Pipelines.ImportItems
{
    public class ReadMapInfo : ImportItemsProcessor
    {
        public override void Process(ImportItemsArgs args)
        {
            Log.Info("EzImporter:Processing import map...", this);
            args.ImportData.Columns.Clear();
            foreach (var column in args.Map.InputFields)
            {
                args.ImportData.Columns.Add(column.Name, typeof(string));
            }
            Log.Info(string.Format("EzImporter:{0} Columns defined in map.", args.Map.InputFields.Count), this);
        }
    }
}