using EzImporter.Import.Item;
using System.Data;

namespace EzImporter.DataReaders
{
    public interface IDataReader
    {
        void ReadData(ref DataTable dataTable, ItemImportTaskArgs args);
        string[] GetColumnNames(ItemImportTaskArgs args);
    }
}
