using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;
using EzImporter.CustomItems;
using Sitecore.Globalization;
using EzImporter.Extensions;

namespace EzImporter.CustomItems.ImportModule
{
public partial class InputColumnCollectionItem : CustomItem
{

public static readonly string TemplateId = "{1A50AD7B-C6C8-4B6D-991B-37885A662DF1}";


#region Boilerplate CustomItem Code

public InputColumnCollectionItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator InputColumnCollectionItem(Item innerItem)
{
return innerItem != null ? new InputColumnCollectionItem(innerItem) : null;
}

public static implicit operator Item(InputColumnCollectionItem customItem)
{
return customItem != null ? customItem.InnerItem : null;
}

public static bool IsValid(Item item)
{
  return item != null && item.IsDerivedFrom(TemplateId);
}

public bool IsValidInputColumnCollection()
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

public CustomTextField Delimiter
{
get
{ 
return new CustomTextField(InnerItem, InnerItem.Fields[DelimiterFieldName]);
}
}
public string DelimiterFieldName = "Delimiter";
public const string DelimiterConstFieldName = "Delimiter";


#endregion //Field Instance Methods
}
}
