using EzImporter.Import.Item;
using EzImporter.Pipelines.ImportItems;

namespace EzImporter.DataReaders
{
    public interface IDataReader
    {
        void ReadData(ImportItemsArgs args);
        string[] GetColumnNames(ImportItemsArgs args);
    }
}
