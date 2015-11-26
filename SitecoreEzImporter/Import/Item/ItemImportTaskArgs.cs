using Sitecore.Data;
using Sitecore.Globalization;

namespace EzImporter.Import.Item
{
    public class ItemImportTaskArgs
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public Database Database { get; set; }
        public ID RootItemId { get; set; }
        public Language TargetLanguage { get; set; }
        public ItemImportMap Map { get; set; }
        public ExistingItemHandling ExistingItemHandling { get; set; }
    }
}
