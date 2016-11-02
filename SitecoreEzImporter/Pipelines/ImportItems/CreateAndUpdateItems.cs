using EzImporter.FieldUpdater;
using EzImporter.Map;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Linq;

namespace EzImporter.Pipelines.ImportItems
{
    public class CreateAndUpdateItems : ImportItemsProcessor
    {
        public override void Process(ImportItemsArgs args)
        {
            var originalIndexingSetting = Sitecore.Configuration.Settings.Indexing.Enabled;
            Sitecore.Configuration.Settings.Indexing.Enabled = false;
            using (new BulkUpdateContext())
            {
                using (new LanguageSwitcher(args.TargetLanguage))
                {
                    var parentItem = args.Database.GetItem(args.RootItemId);
                    foreach (var importItem in args.ImportItems)
                    {
                        ImportItems(args, importItem, parentItem, true);
                    }
                }
            }
            Sitecore.Configuration.Settings.Indexing.Enabled = originalIndexingSetting;
        }

        private void ImportItems(ImportItemsArgs args, ItemDto importItem, Item parentItem,
            bool rootLevel)
        {
            if (rootLevel ||
                importItem.Parent.Name == parentItem.Name)
            {
                var createdItem = CreateItem(args, importItem, parentItem);
                if (createdItem != null
                    && importItem.Children != null
                    && importItem.Children.Any())
                {
                    foreach (var childImportItem in importItem.Children)
                    {
                        ImportItems(args, childImportItem, createdItem, false);
                    }
                }
            }
        }

        private Item CreateItem(ImportItemsArgs args, ItemDto importItem, Item parentItem)
        {
            //CustomItemBase nItemTemplate = GetNewItemTemplate(dataRow);
            var templateItem = args.Database.GetTemplate(importItem.TemplateId);

            //get the parent in the specific language
            Item parent = args.Database.GetItem(parentItem.ID);

            Item item;
            //search for the child by name
            item = parent.GetChildren()[importItem.Name];
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
                    args.Statistics.UpdatedItems++;
                }
            }
            else
            {
                //if not found then create one
                args.Statistics.CreatedItems++;
                item = parent.Add(importItem.Name, templateItem);
                Log.Info(string.Format("EzImporter:Creating item {0}", item.Paths.ContentPath), this);
            }

            if (item == null)
            {
                throw new NullReferenceException("the new item created was null");
            }

            using (new EditContext(item, true, false))
            {
                //add in the field mappings
                foreach (var key in importItem.Fields.Keys)
                {
                    var fieldValue = importItem.Fields[key];
                    var field = item.Fields[key];
                    if (field != null)
                    {
                        FieldUpdateManager.UpdateField(field, fieldValue, args.ImportOptions);
                        Log.Info(string.Format("'{0}' field set to '{1}'", key, fieldValue), this);
                    }
                    else
                    {
                        Log.Info(
                            string.Format(
                                "EzImporter:Field '{0}' not found on item, skipping update for this field",
                                key), this);
                    }
                }
                return item;
            }
        }
    }
}