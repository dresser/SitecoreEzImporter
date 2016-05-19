using EzImporter.Import.Item;
using EzImporter.Pipelines.ImportItems;
using Sitecore.Diagnostics;
using System;
using System.IO;

namespace EzImporter.DataReaders
{
    public class CsvDataReader : IDataReader
    {
        public void ReadData(ImportItemsArgs args)
        {
            Log.Info("EzImporter:Reading CSV input data...", this);
            try
            {
                var reader = new StreamReader(args.FileStream);
                var lineCount = 0;
                do
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        var row = args.ImportData.NewRow();
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
                        args.ImportData.Rows.Add(row);
                        lineCount++;
                    }
                } while (!reader.EndOfStream);
                Log.Info(string.Format("EzImporter:{0} records read from input data.", lineCount), this);
            }
            catch (Exception ex)
            {
                Log.Error("EzImporter:" + ex.ToString(), this);
            }
        }


        public string[] GetColumnNames(ImportItemsArgs args)
        {
            Log.Info("EzImporter:Reading column names from input CSV file...", this);
            try
            {
                using (var reader = new StreamReader(args.FileStream))
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        return line.Split(args.ImportOptions.CsvDelimiter, StringSplitOptions.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("EzImporter:" + ex.ToString(), this);
            }
            return new string[] {};
        }
    }
}