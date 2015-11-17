using Sitecore.Data;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EzImporter
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