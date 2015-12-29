using System.Collections.Generic;

namespace EzImporter.Map
{
    public class ItemImportMap
    {
        public char[] CsvDelimiter { get; set; }
        public List<InputField> InputFields { get; set; }
        public List<OutputMap> OutputMaps { get; set; }
    }
}
