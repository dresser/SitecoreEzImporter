using EzImporter.Import.Item;
using System;
using System.IO;
using System.Text;

namespace EzImporter.DataReaders
{
    public class CsvDataReader : IDataReader
    {
        public void ReadData(ref System.Data.DataTable dataTable, ItemImportTaskArgs args, StringBuilder log)
        {
            log.AppendLine("Reading CSV input data...");
            try
            {
                var reader = new StreamReader(args.FileStream);
                var lineCount = 0;
                do
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        var row = dataTable.NewRow();
                        var values = line.Split(args.ImportOptions.CsvDelimiter, StringSplitOptions.None);
                        for (int j = 0; j < args.Map.InputFields.Count; j++)
                        {
                            if (j < values.Length)
                            {
                                row[j] = values[j];
                            }
                            else
                            {
                                row[j] = "";
                            }
                        }
                        dataTable.Rows.Add(row);
                        lineCount++;
                    }
                } while (!reader.EndOfStream);
                log.AppendFormat("{0} records read from input data. {1}", lineCount, Environment.NewLine);
            }
            catch (Exception ex)
            {
                log.AppendLine(ex.ToString());
            }
        }
    }
}