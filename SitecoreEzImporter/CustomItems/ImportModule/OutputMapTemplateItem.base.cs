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
public partial class OutputMapTemplateItem : CustomItem
{

public static readonly string TemplateId = "{58623B95-07DC-4F00-8D7A-337BF116FB54}";


#region Boilerplate CustomItem Code

public OutputMapTemplateItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator OutputMapTemplateItem(Item innerItem)
{
return innerItem != null ? new OutputMapTemplateItem(innerItem) : null;
}

public static implicit operator Item(OutputMapTemplateItem customItem)
{
return customItem != null ? customItem.InnerItem : null;
}

public static bool IsValid(Item item)
{
  return item != null && item.IsDerivedFrom(TemplateId);
}

public bool IsValidOutputMapTemplate()
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

public CustomLookupField TargetTemplate
{
get
{ 
return new CustomLookupField(InnerItem, InnerItem.Fields[TargetTemplateFieldName]);
}
}
public string TargetTemplateFieldName = "TargetTemplate";
public const string TargetTemplateConstFieldName = "TargetTemplate";


public CustomLookupField ItemNameField
{
get
{ 
return new CustomLookupField(InnerItem, InnerItem.Fields[ItemNameFieldFieldName]);
}
}
public string ItemNameFieldFieldName = "ItemNameField";
public const string ItemNameFieldConstFieldName = "ItemNameField";


#endregion //Field Instance Methods
}
}
