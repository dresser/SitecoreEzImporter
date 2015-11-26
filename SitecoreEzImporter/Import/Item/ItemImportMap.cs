using System.Collections.Generic;
using System.Linq;
using EzImporter.Extensions;
using Sitecore.Data;

namespace EzImporter.Import.Item
{
    public class ItemImportMap
    {
        public char[] CsvDelimiter { get; set; }
        public List<InputField> InputFields { get; set; }
        public List<OutputMap> OutputMaps { get; set; }
        private static readonly ID OutputMapTemplateId = new ID(CustomItems.ImportModule.OutputMapTemplateItem.TemplateId);
        private static readonly ID FieldCollectionTemplateId = new ID(CustomItems.ImportModule.OutputFieldCollectionItem.TemplateId);
        private static readonly ID OutputFieldTemplateId = new ID(CustomItems.ImportModule.OutputFieldItem.TemplateId);

        public static ItemImportMap BuildMapInfo(ID mapId)
        {
            var database = Sitecore.Configuration.Factory.GetDatabase("master");
            var mapItem = database.GetItem(mapId);
            var inputColumnsItem =
                mapItem.FirstChildInheritingFrom(new ID(CustomItems.ImportModule.InputColumnCollectionItem.TemplateId));

            var mapInfo = new ItemImportMap();
            mapInfo.CsvDelimiter =
                inputColumnsItem[CustomItems.ImportModule.InputColumnCollectionItem.DelimiterConstFieldName].ToCharArray();
            mapInfo.InputFields = inputColumnsItem.Children.Select(c => new InputField { Name = c.Name }).ToList();

            mapInfo.OutputMaps = new List<OutputMap>();
            var outputMapItems = mapItem.Children.Where(c => c.InheritsFrom(OutputMapTemplateId));
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
            var outputMapCustomItem = new CustomItems.ImportModule.OutputMapTemplateItem(item);
            outputMap.TemplateId = outputMapCustomItem.TargetTemplate.Item.ID;
            outputMap.NameInputField = outputMapCustomItem.ItemNameField.Item.Name;
            var fieldsCollection = item.Children.FirstOrDefault(c => c.InheritsFrom(FieldCollectionTemplateId));
            if (fieldsCollection != null)
            {
                foreach (var field in fieldsCollection.Children.Where(c => c.InheritsFrom(OutputFieldTemplateId)))
                {
                    var fieldCustomItem = new CustomItems.ImportModule.OutputFieldItem(field);
                    outputMap.Fields.Add(new OutputField
                    {
                        SourceColumn = fieldCustomItem.InputField.Item.Name,
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
            if (parentMap != null && !outputMap.Fields.Any(f => f.SourceColumn == parentMap.NameInputField))
            {
                outputMap.Fields.Add(new OutputField
                {
                    SourceColumn = parentMap.NameInputField,
                    TargetFieldName = ""
                });
            }

            var childMapItems = item.Children.Where(c => c.InheritsFrom(OutputMapTemplateId));
            if (childMapItems != null && childMapItems.Any())
            {
                foreach (var childMapItem in childMapItems)
                {
                    outputMap.ChildMaps.Add(CreateOutputMap(childMapItem, outputMap));
                }
            }

            return outputMap;
        }
    }
}
