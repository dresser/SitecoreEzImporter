using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using EzImporter.Import.Item;

namespace EzImporter.DataReaders
{
    public interface IDataReader
    {
        void ReadData(ref DataTable dataTable, ItemImportTaskArgs args, StringBuilder log);
    }
}
