using System.Linq;
using EzImporter.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;

namespace EzImporter.FieldUpdater
{
    public class CheckBoxListFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Field field, string importValue, IImportOptions importOptions)
        {
            var separator = new[] {importOptions.MultipleValuesImportSeparator};
            var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
            if (selectionSource != null)
            {
                var importValues = importValue != null
                    ? importValue.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                    : new string[] {};
                var idListValue = "";
                foreach (var value in importValues)
                {
                    var selectedItem = selectionSource.Children[value];
                    if (selectedItem != null)
                    {
                        idListValue += "|" + selectedItem.ID;
                    }
                    else
                    {
                        if (importOptions.InvalidLinkHandling == InvalidLinkHandling.CreateItem)
                        {
                            var firstChild = selectionSource.Children.FirstOrDefault();
                            if (firstChild != null)
                            {
                                var template = field.Item.Database.GetTemplate(firstChild.TemplateID);
                                var itemName = Utils.GetValidItemName(value);
                                var createdItem = selectionSource.Add(itemName, template);
                                if (createdItem != null)
                                {
                                    idListValue += "|" + createdItem.ID.ToString();
                                }
                            }
                        }
                        else if (importOptions.InvalidLinkHandling == InvalidLinkHandling.SetBroken)
                        {
                            idListValue += "|" + value;
                        }
                    }
                }
                if (idListValue.StartsWith("|"))
                {
                    idListValue = idListValue.Substring(1);
                }
                field.Value = idListValue;
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
        }
    }
}