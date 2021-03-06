﻿using EzImporter.Extensions;
using EzImporter.Map;
using System;
using System.Data;
using System.Linq;

namespace EzImporter.Pipelines.ImportItems
{
    public class BuildImportDataStructure : ImportItemsProcessor
    {
        public override void Process(ImportItemsArgs args)
        {
            var rootItem = new ItemDto("<root>"); //ick
            foreach (var outputMap in args.Map.OutputMaps)
            {
                ImportMapItems(args, args.ImportData, outputMap, rootItem, true); //ick
            }
            args.ImportItems.AddRange(rootItem.Children); //ick
        }

        private void ImportMapItems(ImportItemsArgs args, DataTable dataTable, OutputMap outputMap, ItemDto parentItem,
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
                    createdItem.Parent = parentItem;
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

        private ItemDto CreateItem(DataRow dataRow, OutputMap outputMap)
        {
            var itemName = Convert.ToString(dataRow[outputMap.NameInputField]);
            var item = new ItemDto(itemName)
            {
                TemplateId = outputMap.TemplateId
            };
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