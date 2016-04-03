using EzImporter.Configuration;
using EzImporter.Map;
using Sitecore.Data;
using Sitecore.Globalization;
using System.IO;

namespace EzImporter.Import.Item
{
    public class ItemImportTaskArgs
    {
        public string FileExtension { get; set; }
        public Stream FileStream { get; set; }
        public Database Database { get; set; }
        public ID RootItemId { get; set; }
        public Language TargetLanguage { get; set; }
        public ItemImportMap Map { get; set; }
        public IImportOptions ImportOptions { get; set; }
    }
}
