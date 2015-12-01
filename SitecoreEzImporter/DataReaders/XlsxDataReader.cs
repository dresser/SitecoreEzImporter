using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using Excel;
using System.Data;
using EzImporter.Import.Item;

namespace EzImporter.DataReaders
{
    public class XlsxDataReader : IDataReader
    {
        public void ReadData(ref System.Data.DataTable dataTable, ItemImportTaskArgs args, StringBuilder log)
        {
            log.AppendLine("Reading XSLX input data");
            try
            {
                FileStream stream = File.Open(args.FileName, FileMode.Open, FileAccess.Read);

                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                excelReader.IsFirstRowAsColumnNames = true;
                if (!excelReader.IsValid)
                {
                    log.AppendLine("Invalid Excel file '" + excelReader.ExceptionMessage + "'");
                    return;
                }
                DataSet result = excelReader.AsDataSet();
                if (result == null)
                {
                    log.AppendLine("No data could be retrieved from Excel file.");
                }
                if (result.Tables == null || result.Tables.Count == 0)
                {
                    log.AppendLine("No worksheets found in Excel file");
                    return;
                }
                var readDataTable = result.Tables[0];
                foreach (var readDataRow in readDataTable.AsEnumerable())
                {
                    var row = dataTable.NewRow();
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
                    dataTable.Rows.Add(row);
                }
                log.AppendFormat("{0} records read from input data. {1}", readDataTable.Rows.Count, Environment.NewLine);
            }
            catch (Exception ex)
            {
                log.AppendLine(ex.ToString());
            }
        }
    }
}