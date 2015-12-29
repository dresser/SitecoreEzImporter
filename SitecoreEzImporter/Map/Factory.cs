using System;
using System.Collections.Generic;
using System.Linq;
using EzImporter.Extensions;
using EzImporter.Import;
using EzImporter.Map.CustomItems;
using Sitecore.Data;

namespace EzImporter.Map
{
    public class Factory
    {
        public static ItemImportMap BuildMapInfo(ID mapId)
        {
            var database = Sitecore.Configuration.Factory.GetDatabase("master");
            var mapItem = database.GetItem(mapId);
            var inputColumnsItem =
                mapItem.FirstChildInheritingFrom(InputColumnCollectionItem.TemplateId);

            var mapInfo = new ItemImportMap();
            mapInfo.CsvDelimiter =
                inputColumnsItem[InputColumnCollectionItem.DelimiterConstFieldName].ToCharArray(); //TODO delimiter should come from settings
            mapInfo.InputFields = inputColumnsItem.Children.Select(c => new InputField { Name = c.Name }).ToList();

            mapInfo.OutputMaps = new List<OutputMap>();
            var outputMapItems = mapItem.Children.Where(c => c.InheritsFrom(OutputMapTemplateItem.TemplateId));
            foreach (var outputMapItem in outputMapItems)
            {
                mapInfo.OutputMaps.Add(CreateOutputMap(outputMapItem, null));
            }
            return mapInfo;
        }

        private static OutputMap CreateOutputMap(Sitecore.Data.Items.Item item, OutputMap parentMap)
        {
            var outputMap = new OutputMap();
            outputMap.ParentMap = parentMap;
            var outputMapCustomItem = new OutputMapTemplateItem(item);
            outputMap.TemplateId = outputMapCustomItem.TargetTemplate.ID;
            outputMap.NameInputField = outputMapCustomItem.ItemNameField.Name;
            var fieldsCollection =
                item.Children.FirstOrDefault(c => c.InheritsFrom(OutputFieldCollectionItem.TemplateId));
            if (fieldsCollection != null)
            {
                foreach (var field in fieldsCollection.Children.Where(c => c.InheritsFrom(OutputFieldItem.TemplateId)))
                {
                    var fieldCustomItem = new OutputFieldItem(field);
                    outputMap.Fields.Add(new OutputField
                    {
                        SourceColumn = fieldCustomItem.InputField.Name,
                        TargetFieldName = fieldCustomItem.Name
                    });
                }
            }
            if (!outputMap.Fields.Any())
            {
                outputMap.Fields.Add(new OutputField
                {
                    SourceColumn = outputMap.NameInputField,
                    TargetFieldName = ""
                });
            }
            if (parentMap != null &&
                !outputMap.Fields.Any(f => f.SourceColumn == parentMap.NameInputField))
            {
                outputMap.Fields.Add(new OutputField
                {
                    SourceColumn = parentMap.NameInputField,
                    TargetFieldName = ""
                });
            }

            var childMapItems = item.Children.Where(c => c.InheritsFrom(OutputMapTemplateItem.TemplateId));
            if (childMapItems != null &&
                childMapItems.Any())
            {
                foreach (var childMapItem in childMapItems)
                {
                    outputMap.ChildMaps.Add(CreateOutputMap(childMapItem, outputMap));
                }
            }

            return outputMap;
        }

        public static MediaImportMap GetMediaImportMap(ID mapId, Database database)
        {
            var mediaMapItem = database.GetItem(mapId);
            var mediaMapCustomItem = new MediaImportMapItem(mediaMapItem);
            var mediaImportMap = new MediaImportMap
            {
                TemplateId = mediaMapCustomItem.TargetTemplate.ID,
                InputFileNameFormat = mediaMapCustomItem.InputFilenameFormat,
                ItemIdProperty = mediaMapCustomItem.ItemIdProperty,
                ImageFieldProperty = mediaMapCustomItem.ImageFieldProperty,
                UseFileNameForMediaItem = mediaMapCustomItem.UseFileNameForMediaItem,
                MediaItemNameFormat = mediaMapCustomItem.NewMediaItemNameFormat,
                AltTextFormat = mediaMapCustomItem.AltTextFormat
            };

            var inputFileNameFormatNoExtension = mediaImportMap.InputFileNameFormat;
            var i = mediaImportMap.InputFileNameFormat.LastIndexOf(".");
            if (i > -1)
            {
                inputFileNameFormatNoExtension = mediaImportMap.InputFileNameFormat.Substring(0, i);
            }
            mediaImportMap.MappingFields = inputFileNameFormatNoExtension.Split(MediaImportMap.FileNameFormatDelimiter, StringSplitOptions.RemoveEmptyEntries);
            if (mediaImportMap.MappingFields.Contains(mediaImportMap.ItemIdProperty) &&
                mediaImportMap.MappingFields.Contains(mediaImportMap.ImageFieldProperty))
            {
                mediaImportMap.IsValid = true;
            }

            if (!mediaImportMap.UseFileNameForMediaItem)
            {
                var mediaItemNameFormatNoExtension = mediaImportMap.MediaItemNameFormat;
                var j = mediaItemNameFormatNoExtension.LastIndexOf(".");
                if (j > -1)
                {
                    mediaItemNameFormatNoExtension = mediaItemNameFormatNoExtension.Substring(0, j);
                }
                mediaImportMap.MediaNameMappingFields = mediaItemNameFormatNoExtension.Split(MediaImportMap.FileNameFormatDelimiter, StringSplitOptions.RemoveEmptyEntries);
                //foreach (var nameMappingField in MediaNameMappingFields)
                //{
                //if (!MappingFields.Contains(nameMappingField))
                //{
                //  IsValid = false;
                //}
                //}
            }

            mediaImportMap.AltTextMappingFields = mediaImportMap.AltTextFormat.Split(MediaImportMap.FileNameFormatDelimiter, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var altTextMappingField in AltTextMappingFields)
            //{
            //    if (!MappingFields.Contains(altTextMappingField))
            //    {
            //        IsValid = false;
            //    }
            //}

            return mediaImportMap;
        }
    }
}