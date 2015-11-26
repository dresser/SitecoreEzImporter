using System;
using Sitecore.Data.Fields;

namespace EzImporter.FieldUpdater
{
    public class DropLinkFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Field field, string importValue)
        {
            try
            {
                var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
                var selectedItem = selectionSource.Children[importValue];
                field.Value = selectedItem.ID.ToString();
            }
            catch (Exception ex)
            {
                if (Settings.InvalidLinkHandling == InvalidLinkHandling.SetBroken)
                {
                    field.Value = importValue;
                }
                else if (Settings.InvalidLinkHandling == InvalidLinkHandling.SetEmpty)
                {
                    field.Value = string.Empty;
                }
            }
        }
    }
}