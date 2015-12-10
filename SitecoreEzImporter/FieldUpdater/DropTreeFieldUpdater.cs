using EzImporter.Configuration;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EzImporter.FieldUpdater
{
    public class DropTreeFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Sitecore.Data.Fields.Field field, string importValue, IImportOptions importOptions)
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