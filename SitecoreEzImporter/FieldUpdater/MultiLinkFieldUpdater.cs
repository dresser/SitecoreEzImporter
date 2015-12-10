using EzImporter.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;

namespace EzImporter.FieldUpdater
{
    public class MultiLinkFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Field field, string importValue, IImportOptions importOptions)
        {
            try
            {
                var separator = new[] {importOptions.MultipleValuesImportSeparator};
                var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
                var importValues = importValue != null
                    ? importValue.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                    : new string[] {};
                var idListValue = "";
                foreach (var value in importValues)
                {
                    var selectedItem = selectionSource != null ? selectionSource.Children[value] : (Item) null;
                    if (selectedItem != null)
                    {
                        idListValue += "|" + selectedItem.ID;
                    }
                    else
                    {
                        if (importOptions.InvalidLinkHandling == InvalidLinkHandling.SetBroken)
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
            }
            catch (Exception ex)
            {
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
}