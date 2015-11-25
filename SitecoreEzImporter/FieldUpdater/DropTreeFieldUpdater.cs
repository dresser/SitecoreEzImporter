using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EzImporter.FieldUpdater
{
    public class DropTreeFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Sitecore.Data.Fields.Field field, string importValue)
        {
            try
            {
                var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
                var selectionItems = new List<Item>(new[] {selectionSource});
                selectionItems.AddRange(selectionSource.Axes.GetDescendants());
                var selectedItem =
                    selectionItems.FirstOrDefault(
                        i => i.Name.Equals(importValue, StringComparison.CurrentCultureIgnoreCase));
                if (selectedItem != null)
                {
                    field.Value = selectedItem.ID.ToString();
                    return;
                }
            }
            catch
            {
            }
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