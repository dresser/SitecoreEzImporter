using EzImporter.Extensions;
using EzImporter.FieldUpdater;
using EzImporter.Map;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Data;
using System.Linq;

namespace EzImporter.Pipelines.ImportItems
{
    public class BuildImportDataStructure : ImportItemsProcessor
    {
        public override void Process(ImportItemsArgs args)
        {
            var rootItem = new ImportItem("<root>"); //ick
            foreach (var outputMap in args.Map.OutputMaps)
            {
                ImportMapItems(args, args.ImportData, outputMap, rootItem, true); //ick
            }
            args.ImportItems.AddRange(rootItem.Children); //ick
        }

        private void ImportMapItems(ImportItemsArgs args, DataTable dataTable, OutputMap outputMap, ImportItem parentItem,
            bool rootLevel)
        {
            var groupedTable = dataTable.GroupBy(outputMap.Fields.Select(f => f.SourceColumn).ToArray());
            for (int i = 0; i < groupedTable.Rows.Count; i++)
            {
                var row = groupedTable.Rows[i];
                if (rootLevel ||
                    Convert.ToString(row[outputMap.ParentMap.NameInputField]) == parentItem.Name)
                {
                    var createdItem = CreateItem(row, outputMap);
                    parentItem.Children.Add(createdItem);
                    if (outputMap.ChildMaps != null
                        && outputMap.ChildMaps.Any())
                    {
                        foreach (var childMap in outputMap.ChildMaps)
                        {
                            ImportMapItems(args, dataTable, childMap, createdItem, false);
                        }
                    }
                }
            }
        }

        private ImportItem CreateItem(DataRow dataRow, OutputMap outputMap)
        {
            string itemName = Utils.GetValidItemName(dataRow[outputMap.NameInputField]);
            var item = new ImportItem(itemName);
            for (int i = 0; i < outputMap.Fields.Count; i++)
            {
                var mapFieldName = outputMap.Fields[i].TargetFieldName;
                if (!string.IsNullOrEmpty(mapFieldName))
                {
                    var fieldValue = dataRow[outputMap.Fields[i].SourceColumn].ToString();
                    item.Fields.Add(mapFieldName, fieldValue);
                }
            }
            return item;
        }
    }
}