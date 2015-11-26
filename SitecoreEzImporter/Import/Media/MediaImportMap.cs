using System;
using System.Linq;
using EzImporter.CustomItems.ImportModule;
using Sitecore.Data;

namespace EzImporter.Import.Media
{
    public class MediaImportMap
    {
        public ID TemplateId { get; set; }
        public string InputFileNameFormat { get; set; }
        public string[] MappingFields { get; protected set; }
        public string ItemIdProperty { get; set; }
        public string ImageFieldProperty { get; set; }
        public bool UseFileNameForMediaItem { get; set; }
        public string MediaItemNameFormat { get; set; }
        public string[] MediaNameMappingFields { get; protected set; }
        public string AltTextFormat { get; set; }
        public string[] AltTextMappingFields { get; protected set; }
        public bool IsValid { get; protected set; }

        public const string FileNameWordDelimiter = "_";
        public static readonly string[] FileNameFormatDelimiter = new[] { FileNameWordDelimiter };

        public const string AltTextWordDelimiter = " ";
        public static readonly string[] AltTextFormatDelimiter = new[] { AltTextWordDelimiter }; 

        public MediaImportMap(ID mapId, Database database)
        {
            var mediaMapItem = database.GetItem(mapId);
            var mediaMapCustomItem = new MediaImportMapItem(mediaMapItem);
            TemplateId = mediaMapCustomItem.TargetTemplate.Item.ID;
            InputFileNameFormat = mediaMapCustomItem.InputFilenameFormat.Raw;
            ItemIdProperty = mediaMapCustomItem.ItemIdProperty.Raw;
            ImageFieldProperty = mediaMapCustomItem.ImageFieldProperty.Raw;
            UseFileNameForMediaItem = mediaMapCustomItem.UseFileNameForMediaItem.Checked;
            MediaItemNameFormat = mediaMapCustomItem.NewMediaItemNameFormat.Raw;
            AltTextFormat = mediaMapCustomItem.AltTextFormat.Raw;

            var inputFileNameFormatNoExtension = InputFileNameFormat;
            var i = InputFileNameFormat.LastIndexOf(".");
            if (i > -1)
            {
                inputFileNameFormatNoExtension = InputFileNameFormat.Substring(0, i);
            }
            MappingFields = inputFileNameFormatNoExtension.Split(FileNameFormatDelimiter, StringSplitOptions.RemoveEmptyEntries);
            if (MappingFields.Contains(ItemIdProperty) &&
                MappingFields.Contains(ImageFieldProperty))
            {
                IsValid = true;
            }

            if (!UseFileNameForMediaItem)
            {
                var mediaItemNameFormatNoExtension = MediaItemNameFormat;
                var j = mediaItemNameFormatNoExtension.LastIndexOf(".");
                if (j > -1)
                {
                    mediaItemNameFormatNoExtension = mediaItemNameFormatNoExtension.Substring(0, j);
                }
                MediaNameMappingFields = mediaItemNameFormatNoExtension.Split(FileNameFormatDelimiter, StringSplitOptions.RemoveEmptyEntries);
                //foreach (var nameMappingField in MediaNameMappingFields)
                //{
                    //if (!MappingFields.Contains(nameMappingField))
                    //{
                      //  IsValid = false;
                    //}
                //}
            }

            AltTextMappingFields = AltTextFormat.Split(FileNameFormatDelimiter, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var altTextMappingField in AltTextMappingFields)
            //{
            //    if (!MappingFields.Contains(altTextMappingField))
            //    {
            //        IsValid = false;
            //    }
            //}
        }
    }

}