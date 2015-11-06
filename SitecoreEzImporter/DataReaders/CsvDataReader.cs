using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;

namespace EzImporter.DataReaders
{
    public class CsvDataReader : IDataReader
    {
        public void ReadData(ref System.Data.DataTable dataTable, ItemImportTaskArgs args, StringBuilder log)
        {
            log.AppendLine("Reading CSV input data...");
            try
            {
                var lines = File.ReadAllLines(args.FileName, System.Text.Encoding.UTF8);
                if (lines.Length == 0)
                {
                    log.AppendLine("No data found in file");
                }
                for (int i = 0; i < lines.Length; i++)
                {
                    var row = dataTable.NewRow();
                    var values = lines[i].Split(args.Map.CsvDelimiter, StringSplitOptions.None);
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
                }
                log.AppendFormat("{0} records read from input data. {1}", lines.Length, Environment.NewLine);
            }
            catch (Exception ex)
            {
                log.AppendLine(ex.ToString());
            }
        }
    }
}