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
            var selectionSource = field.Item.Database.SelectSingleItem(field.Source);
            if (selectionSource != null)
            {
                var query = "." +
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