using Sitecore.Data;

namespace EzImporter.Map
{
    public class MediaImportMap
    {
        public ID TemplateId { get; set; }
        public string InputFileNameFormat { get; set; }
        public string[] MappingFields { get; set; }
        public string ItemIdProperty { get; set; }
        public string ImageFieldProperty { get; set; }
        public bool UseFileNameForMediaItem { get; set; }
        public string MediaItemNameFormat { get; set; }
        public string[] MediaNameMappingFields { get; set; }
        public string AltTextFormat { get; set; }
        public string[] AltTextMappingFields { get; set; }
        public bool IsValid { get; set; }

        public const string FileNameWordDelimiter = "_";
        public static readonly string[] FileNameFormatDelimiter = new[] { FileNameWordDelimiter };

        public const string AltTextWordDelimiter = " ";
        public static readonly string[] AltTextFormatDelimiter = new[] { AltTextWordDelimiter }; 
    }
}