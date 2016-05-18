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
        protected ItemImportTaskArgs Args { get; set; }
        //protected StringBuilder Log { get; set; }

        public ItemImportTask()
        {
            //Log = new StringBuilder();
        }

        public void Run(ItemImportTaskArgs args)
        {
            Args = args;
            ValidateArgs();
            var dataTable = new DataTable();
            ReadMapInfo(ref dataTable);
            ReadData(ref dataTable);
            ImportItems(dataTable);
        }

        protected bool ValidateArgs()
        {
            Log.Info("EzImporter:Validating input...", this);
            var argsValid = true;
            if (Args.FileStream == null)
            {
                Log.Error("EzImporter:Input file not found.", this);
                argsValid = false;
            }
            return argsValid;
        }

        protected void ReadMapInfo(ref DataTable dataTable)
        {
            Log.Info("EzImporter:Processing import map...", this);
            dataTable.Columns.Clear();
            foreach (var column in Args.Map.InputFields)
            {
                dataTable.Columns.Add(column.Name, typeof (string));
            }
            Log.Info(string.Format("EzImporter:{0} Columns defined in map.", Args.Map.InputFields.Count), this);
        }

        protected void ReadData(ref DataTable dataTable)
        {
            DataReaders.IDataReader reader;
            if (Args.FileExtension == "csv")
            {
                reader = new DataReaders.CsvDataReader();
            }
            else if (Args.FileExtension == "xlsx" ||
                     Args.FileExtension == "xls")
            {
                reader = new DataReaders.XlsxDataReader();
            }
            else
            {
                Log.Info("EzImporter:Unsupported file format supplied. DataImporter accepts *.CSV and *.XLSX files",
                    this);
                return;
            }
            reader.ReadData(ref dataTable, Args);
            Args.Statistics.InputDataRows = dataTable.Rows.Count;
        }

        protected void ImportItems(DataTable dataTable)
        {
            using (new LanguageSwitcher(Args.TargetLanguage))
            {
                var parentItem = Args.Database.GetItem(Args.RootItemId);
                foreach (var outputMap in Args.Map.OutputMaps)
                {
                    ImportMapItems(dataTable, outputMap, parentItem, true);
                }
            }
        }

        private void ImportMapItems(DataTable dataTable, OutputMap outputMap, Sitecore.Data.Items.Item parentItem,
            bool rootLevel)
        {
            var groupedTable = dataTable.GroupBy(outputMap.Fields.Select(f => f.SourceColumn).ToArray());
            for (int i = 0; i < groupedTable.Rows.Count; i++)
            {
                var row = groupedTable.Rows[i];
                if (rootLevel ||
                    Convert.ToString(row[outputMap.ParentMap.NameInputField]) == parentItem.Name)
                {
                    var createdItem = CreateItem(row, outputMap, parentItem);
                    if (createdItem != null &&
                        outputMap.ChildMaps != null && outputMap.ChildMaps.Any())
                    {
                        foreach (var childMap in outputMap.ChildMaps)
                        {
                            ImportMapItems(dataTable, childMap, createdItem, false);
                        }
                    }
                }
            }
        }

        protected Sitecore.Data.Items.Item CreateItem(DataRow dataRow, OutputMap outputMap,
            Sitecore.Data.Items.Item parentItem)
        {
            //CustomItemBase nItemTemplate = GetNewItemTemplate(dataRow);
            var templateItem = Args.Database.GetTemplate(outputMap.TemplateId);

            using (new LanguageSwitcher(Args.TargetLanguage))
            {
                //get the parent in the specific language
                Sitecore.Data.Items.Item parent = Args.Database.GetItem(parentItem.ID);

                Sitecore.Data.Items.Item item;
                //search for the child by name
                string itemName = Utils.GetValidItemName(dataRow[outputMap.NameInputField]);
                item = parent.GetChildren()[itemName];
                if (item != null)
                {
                    if (Args.ImportOptions.ExistingItemHandling == ExistingItemHandling.AddVersion)
                    {
                        Args.Statistics.UpdatedItems++;
                        item = item.Versions.AddVersion();
                        Log.Info(string.Format("EzImporter:Creating new version of item {0}", item.Paths.ContentPath),
                            this);
                    }
                    else if (Args.ImportOptions.ExistingItemHandling == ExistingItemHandling.Skip)
                    {
                        Log.Info(string.Format("EzImporter:Skipping update of item {0}", item.Paths.ContentPath), this);
                        return item;
                    }
                    else if (Args.ImportOptions.ExistingItemHandling == ExistingItemHandling.Update)
                    {
                        //continue to update current item/version
                        Args.Statistics.UpdatedItems ++;
                    }
                }
                else
                {
                    //if not found then create one
                    Args.Statistics.CreatedItems++;
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
                                FieldUpdateManager.UpdateField(field, fieldValue, Args.ImportOptions);
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
