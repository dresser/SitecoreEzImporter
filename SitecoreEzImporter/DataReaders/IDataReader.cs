using EzImporter.Import.Item;

namespace EzImporter.DataReaders
{
    public interface IDataReader
    {
        void ReadData(ItemImportTaskArgs args);
        string[] GetColumnNames(ItemImportTaskArgs args);
    }
}
