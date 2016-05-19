using EzImporter.Extensions;
using EzImporter.FieldUpdater;
using EzImporter.Map;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Data;
using System.Linq;

namespace EzImporter.Import.Item
{
    public class ItemImportTask
    {
        public void Run(ItemImportTaskArgs args)
        {
            ValidateArgs(args);
            ReadMapInfo(args);
            ReadData(args);
            ImportItems(args);
        }

        protected bool ValidateArgs(ItemImportTaskArgs args)
        {
            Log.Info("EzImporter:Validating input...", this);
            var argsValid = true;
            if (args.FileStream == null)
            {
                Log.Error("EzImporter:Input file not found.", this);
                argsValid = false;
            }
            return argsValid;
        }

        protected void ReadMapInfo(ItemImportTaskArgs args)
        {
            Log.Info("EzImporter:Processing import map...", this);
            args.ImportData.Columns.Clear();
            foreach (var column in args.Map.InputFields)
            {
                args.ImportData.Columns.Add(column.Name, typeof(string));
            }
            Log.Info(string.Format("EzImporter:{0} Columns defined in map.", args.Map.InputFields.Count), this);
        }

        protected void ReadData(ItemImportTaskArgs args)
        {
            DataReaders.IDataReader reader;
            if (args.FileExtension == "csv")
            {
                reader = new DataReaders.CsvDataReader();
            }
            else if (args.FileExtension == "xlsx" ||
                     args.FileExtension == "xls")
            {
                reader = new DataReaders.XlsxDataReader();
            }
            else
            {
                Log.Info("EzImporter:Unsupported file format supplied. DataImporter accepts *.CSV and *.XLSX files",
                    this);
                return;
            }
            reader.ReadData(args);
            args.Statistics.InputDataRows = args.ImportData.Rows.Count;
        }

        protected void ImportItems(ItemImportTaskArgs args)
        {
            using (new LanguageSwitcher(args.TargetLanguage))
            {
                var parentItem = args.Database.GetItem(args.RootItemId);
                foreach (var outputMap in args.Map.OutputMaps)
                {
                    ImportMapItems(args, args.ImportData, outputMap, parentItem, true);
                }
            }
        }

        private void ImportMapItems(ItemImportTaskArgs args, DataTable dataTable, OutputMap outputMap, Sitecore.Data.Items.Item parentItem,
            bool rootLevel)
        {
            var groupedTable = dataTable.GroupBy(outputMap.Fields.Select(f => f.SourceColumn).ToArray());
            for (int i = 0; i < groupedTable.Rows.Count; i++)
            {
                var row = groupedTable.Rows[i];
                if (rootLevel ||
                    Convert.ToString(row[outputMap.ParentMap.NameInputField]) == parentItem.Name)
                {
                    var createdItem = CreateItem(args, row, outputMap, parentItem);
                    if (createdItem != null &&
                        outputMap.ChildMaps != null && outputMap.ChildMaps.Any())
                    {
                        foreach (var childMap in outputMap.ChildMaps)
                        {
                            ImportMapItems(args, dataTable, childMap, createdItem, false);
                        }
                    }
                }
            }
        }

        protected Sitecore.Data.Items.Item CreateItem(ItemImportTaskArgs args, DataRow dataRow, OutputMap outputMap,
            Sitecore.Data.Items.Item parentItem)
        {
            //CustomItemBase nItemTemplate = GetNewItemTemplate(dataRow);
            var templateItem = args.Database.GetTemplate(outputMap.TemplateId);

            using (new LanguageSwitcher(args.TargetLanguage))
            {
                //get the parent in the specific language
                Sitecore.Data.Items.Item parent = args.Database.GetItem(parentItem.ID);

                Sitecore.Data.Items.Item item;
                //search for the child by name
                string itemName = Utils.GetValidItemName(dataRow[outputMap.NameInputField]);
                item = parent.GetChildren()[itemName];
                if (item != null)
                {
                    if (args.ImportOptions.ExistingItemHandling == ExistingItemHandling.AddVersion)
                    {
                        args.Statistics.UpdatedItems++;
                        item = item.Versions.AddVersion();
                        Log.Info(string.Format("EzImporter:Creating new version of item {0}", item.Paths.ContentPath),
                            this);
                    }
                    else if (args.ImportOptions.ExistingItemHandling == ExistingItemHandling.Skip)
                    {
                        Log.Info(string.Format("EzImporter:Skipping update of item {0}", item.Paths.ContentPath), this);
                        return item;
                    }
                    else if (args.ImportOptions.ExistingItemHandling == ExistingItemHandling.Update)
                    {
                        //continue to update current item/version
                        args.Statistics.UpdatedItems ++;
                    }
                }
                else
                {
                    //if not found then create one
                    args.Statistics.CreatedItems++;
                    item = parent.Add(itemName, templateItem);
                    Log.Info(string.Format("EzImporter:Creating item {0}", item.Paths.ContentPath), this);
                }

                if (item == null)
                {
                    throw new NullReferenceException("the new item created was null");
                }

                using (new EditContext(item, true, false))
                {
                    //add in the field mappings
                    for (int i = 0; i < outputMap.Fields.Count; i++)
                    {
                        var mapFieldName = outputMap.Fields[i].TargetFieldName;
                        if (!string.IsNullOrEmpty(mapFieldName))
                        {
                            var field = item.Fields[mapFieldName];
                            if (field != null)
                            {
                                var fieldValue = dataRow[outputMap.Fields[i].SourceColumn].ToString();
                                FieldUpdateManager.UpdateField(field, fieldValue, args.ImportOptions);
                                Log.Info(string.Format("'{0}' field set to '{1}'", mapFieldName, fieldValue), this);
                            }
                            else
                            {
                                Log.Info(
                                    string.Format(
                                        "EzImporter:Field '{0}' not found on item, skipping update for this field",
                                        mapFieldName), this);
                            }
                        }
                        else
                        {
                            Log.Info("EzImporter:Null or empty mapping field name found", this);
                        }
                    }
                }
                return item;
            }
        }
    }
}
