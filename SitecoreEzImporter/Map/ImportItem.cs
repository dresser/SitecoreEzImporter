using System.Collections.Generic;

namespace EzImporter.Map
{
    public class ImportItem
    {
        public string Name { get; set; }
        public Dictionary<string, string> Fields { get; set; }
        public List<ImportItem> Children { get; set; }

        public ImportItem(string name)
        {
            Name = name;
            Fields = new Dictionary<string, string>();
            Children = new List<ImportItem>();
        }
    }
}