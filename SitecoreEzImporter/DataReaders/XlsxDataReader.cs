using Excel;
using EzImporter.Import.Item;
using Sitecore.Diagnostics;
using System;
using System.Data;
using System.Linq;

namespace EzImporter.DataReaders
{
    public class XlsxDataReader : IDataReader
    {
        public void ReadData(ItemImportTaskArgs args)
        {
            Log.Info("EzImporter:Reading XSLX input data", this);
            try
            {
                IExcelDataReader excelReader;
                if (args.FileExtension == "xls")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(args.FileStream, ReadOption.Loose);
                }
                else
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(args.FileStream);
                }

                excelReader.IsFirstRowAsColumnNames = args.FirstRowAsColumnNames;
                if (!excelReader.IsValid)
                {
                    Log.Error("EzImporter:Invalid Excel file '" + excelReader.ExceptionMessage + "'", this);
                    return;
                }
                DataSet result = excelReader.AsDataSet();
                if (result == null)
                {
                    Log.Error("EzImporter:No data could be retrieved from Excel file.", this);
                }
                if (result.Tables == null || result.Tables.Count == 0)
                {
                    Log.Error("EzImporter:No worksheets found in Excel file", this);
                    return;
                }
                var readDataTable = result.Tables[0];
                foreach (var readDataRow in readDataTable.AsEnumerable())
                {
                    var row = args.ImportData.NewRow();
                    for (int i = 0; i < args.Map.InputFields.Count; i++)
                    {
                        if (i < readDataTable.Columns.Count && readDataRow[i] != null)
                        {
                            row[i] = Convert.ToString(readDataRow[i]);
                        }
                        else
                        {
                            row[i] = "";
                        }
                    }
                    args.ImportData.Rows.Add(row);
                }
                Log.Info(string.Format("EzImporter:{0} records read from input data.", readDataTable.Rows.Count), this);
            }
            catch (Exception ex)
            {
                Log.Error("EzImporter:" + ex.ToString(), this);
            }
        }


        public string[] GetColumnNames(ItemImportTaskArgs args)
        {
            Log.Info("EzImporter:Reading column names from input XSLX file...", this);
            try
            {
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(args.FileStream);

                excelReader.IsFirstRowAsColumnNames = true; //assume first line is data, so we can read it
                if (!excelReader.IsValid)
                {
                    Log.Info("EzImporter:Invalid Excel file '" + excelReader.ExceptionMessage + "'", this);
                    return new string[] {};
                }
                DataSet result = excelReader.AsDataSet();
                if (result == null)
                {
                    Log.Info("EzImporter:No data could be retrieved from Excel file.", this);
                }
                if (result.Tables == null || result.Tables.Count == 0)
                {
                    Log.Info("EzImporter:No worksheets found in Excel file", this);
                    return new string[] {};
                }
                var readDataTable = result.Tables[0];
                return readDataTable.Columns
                    .Cast<DataColumn>()
                    .Select(c => c.ColumnName).ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("EzImporter:" + ex.ToString(), this);
            }
            return new string[] { };
        }
    }
}