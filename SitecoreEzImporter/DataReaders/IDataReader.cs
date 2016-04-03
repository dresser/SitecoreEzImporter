using EzImporter.Import.Item;
using System.Data;
using System.Text;

namespace EzImporter.DataReaders
{
    public interface IDataReader
    {
        void ReadData(ref DataTable dataTable, ItemImportTaskArgs args, StringBuilder log);
        string[] GetColumnNames(ItemImportTaskArgs args, StringBuilder log);
    }
}
