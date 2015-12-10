using System;
using EzImporter.Configuration;
using Sitecore.Data.Fields;

namespace EzImporter.FieldUpdater
{
    public class DropLinkFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Field field, string importValue, IImportOptions importOptions)
        {
            try
            {
                var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
                var selectedItem = selectionSource.Children[importValue];
                field.Value = selectedItem.ID.ToString();
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