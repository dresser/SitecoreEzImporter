using System.Data;
using System.Linq;

namespace EzImporter.Extensions
{
    public static class DataTableExtensions
    {
        public static DataTable GroupBy(this DataTable inputDataTable, string[] columnNames)
        {
            var groupedDataTable = new DataTable();
            foreach (var columnName in columnNames)
            {
                groupedDataTable.Columns.Add(columnName, inputDataTable.Columns[columnName].DataType);
            }
            foreach (var grouping in inputDataTable.AsEnumerable().GroupBy(r => new NTuple<object>(from columnName in columnNames select r[columnName])))
            {
                var row = groupedDataTable.NewRow();
                for (int i = 0; i < columnNames.Length; i++)
                {
                    row[i] = grouping.Key.Values[i];
                }
                groupedDataTable.Rows.Add(row);
            }
            return groupedDataTable;
        }
    }
}