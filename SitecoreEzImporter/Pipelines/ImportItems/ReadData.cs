using Sitecore.Diagnostics;

namespace EzImporter.Pipelines.ImportItems
{
    public class ReadData : ImportItemsProcessor
    {
        public override void Process(ImportItemsArgs args)
        {
            DataReaders.IDataReader reader;
            if (args.FileExtension == "csv")
            {
                reader = new DataReaders.CsvDataReader();
            }
            else if (args.FileExtension == "xlsx" ||
                     args.FileExtension == "xls")
            {
                reader = new DataReaders.XlsxDataReader();
            }
            else
            {
                Log.Info("EzImporter:Unsupported file format supplied. DataImporter accepts *.CSV and *.XLSX files",
                    this);
                return;
            }
            reader.ReadData(args);
            args.Statistics.InputDataRows = args.ImportData.Rows.Count;
        }
    }
}