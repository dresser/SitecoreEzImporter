using Sitecore.Data;
using Sitecore.Globalization;

namespace EzImporter.Import.Media
{
    public class MediaImportTaskArgs
    {
        public string ZipFileName { get; set; }
        public string ExtractionFolder { get; set; }
        public Database Database { get; set; }
        public ID RootMediaItemId { get; set; }
        public ID RootDataItemId { get; set; }
        public Language TargetLanguage { get; set; }
        public MediaImportMap MediaImportMap { get; set; }
        public ExistingItemHandling ExistingItemHandling { get; set; }
    }
}