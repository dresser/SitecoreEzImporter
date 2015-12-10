using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;

namespace EzImporter.FieldUpdater
{
    public class MultiLinkFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Field field, string importValue)
        {
            var settings = Settings.GetConfigurationSettings();
            try
            {
                var separator = new[] {settings.MultipleValuesImportSeparator};
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
                        if (settings.InvalidLinkHandling == InvalidLinkHandling.SetBroken)
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
                if (settings.InvalidLinkHandling == InvalidLinkHandling.SetBroken)
                {
                    field.Value = importValue;
                }
                else if (settings.InvalidLinkHandling == InvalidLinkHandling.SetEmpty)
                {
                    field.Value = string.Empty;
                }
            }
        }
    }
}