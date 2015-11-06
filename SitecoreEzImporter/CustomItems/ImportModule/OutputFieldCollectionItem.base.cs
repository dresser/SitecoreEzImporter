using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Globalization;
using EzImporter.Extensions;

namespace EzImporter.CustomItems.ImportModule
{
public partial class OutputFieldCollectionItem : CustomItem
{

public static readonly string TemplateId = "{475A5BBD-3A5A-47CC-A65D-C1B1AC5773F0}";


#region Boilerplate CustomItem Code

public OutputFieldCollectionItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator OutputFieldCollectionItem(Item innerItem)
{
return innerItem != null ? new OutputFieldCollectionItem(innerItem) : null;
}

public static implicit operator Item(OutputFieldCollectionItem customItem)
{
return customItem != null ? customItem.InnerItem : null;
}

public static bool IsValid(Item item)
{
  return item != null && item.IsDerivedFrom(TemplateId);
}

public bool IsValidOutputFieldCollection()
{
  return InnerItem != null && InnerItem.IsDerivedFrom(TemplateId);
}

public bool HasVersion()
{
    return HasVersion(InnerItem, Sitecore.Context.Language);
}

public bool HasVersion(Item item)
{
    return HasVersion(item, Sitecore.Context.Language);
}

public bool HasVersion(Language language)
{
    return HasVersion(InnerItem, language);
}

public bool HasVersion(Item item, Language language)
{
    Item itemInLanguage = Sitecore.Context.Database.GetItem(item.ID, language);
    return itemInLanguage.Versions.Count > 0;
}

public static bool ItemHasVersion(Item item, Language language)
{
    Item itemInLanguage = Sitecore.Context.Database.GetItem(item.ID, language);
    return itemInLanguage.Versions.Count > 0;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods

// NOTE Fieldnames are transformed, removing spaces and dashes to reduce the risk of illegal variable names.
// NOTE The original fieldnames are, of course, still used in the reference to the actual Sitecore field.
// NOTE To change this behavior, see sitecore modules\Shell\CustomItemGenerator\Nvelocity Templates\CustomItem.base.vm

#endregion //Field Instance Methods
}
}
