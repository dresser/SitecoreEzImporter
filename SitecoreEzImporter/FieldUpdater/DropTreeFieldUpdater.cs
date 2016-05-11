using EzImporter.Configuration;
using Sitecore.Data;

namespace EzImporter.FieldUpdater
{
    public class DropTreeFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Sitecore.Data.Fields.Field field, string importValue, IImportOptions importOptions)
        {
            var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
            if (selectionSource != null)
            {
                var query = ID.IsID(importValue)
                    ? ".//[@@id=\"" + ID.Parse(importValue) + "\"]"
                    : "." +
                      Sitecore.StringUtil.EnsurePrefix('/',
                          importValue.Replace(importOptions.TreePathValuesImportSeparator, "/"));
                var selectedItem = selectionSource.Axes.SelectSingleItem(query);
                if (selectedItem != null)
                {
                    field.Value = selectedItem.ID.ToString();
                    return;
                }
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