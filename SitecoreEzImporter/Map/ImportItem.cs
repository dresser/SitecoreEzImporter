using System.Collections.Generic;
using Sitecore.Data;

namespace EzImporter.Map
{
    public class ImportItem
    {
        public string Name { get; set; }
        public ID TemplateId { get; set; }
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