using EzImporter.Extensions;
using EzImporter.Map.CustomItems;
using Sitecore.Data;
using System.Linq;

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

            var mapInfo = new ItemImportMap
            {
                InputFields = inputColumnsItem.Children.Select(c => new InputField {Name = c.Name}).ToList(),
                OutputMaps = mapItem.Children
                    .Where(c => c.InheritsFrom(OutputMapTemplateItem.TemplateId))
                    .Select(om => CreateOutputMap(om, null))
                    .ToList()
            };
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
    }
}