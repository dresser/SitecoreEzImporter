using System;
using System.Linq;
using EzImporter.Configuration;
using Sitecore.Data.Fields;

namespace EzImporter.FieldUpdater
{
    public class DropLinkFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Field field, string importValue, IImportOptions importOptions)
        {
            var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
            var selectedItem = selectionSource.Children[importValue];
            if (selectedItem != null)
            {
                field.Value = selectedItem.ID.ToString();
                return;
            }
            if (importOptions.InvalidLinkHandling == InvalidLinkHandling.SetBroken)
            {
                field.Value = importValue;
            }
            else if (importOptions.InvalidLinkHandling == InvalidLinkHandling.SetEmpty)
            {
                field.Value = string.Empty;
            }
            else if (importOptions.InvalidLinkHandling == InvalidLinkHandling.CreateItem)
            {
                var firstChild = selectionSource.Children.FirstOrDefault();
                if (firstChild != null)
                {
                    var template = field.Item.Database.GetTemplate(firstChild.TemplateID);
                    var itemName = Utils.GetValidItemName(importValue);
                    selectionSource.Add(itemName, template);
                }
            }
        }
    }
}