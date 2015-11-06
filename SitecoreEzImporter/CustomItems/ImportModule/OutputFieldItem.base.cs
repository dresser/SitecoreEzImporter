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
public partial class OutputFieldItem : CustomItem
{

public static readonly string TemplateId = "{317A4F55-F36E-4E6E-A411-85883BFD4496}";


#region Boilerplate CustomItem Code

public OutputFieldItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator OutputFieldItem(Item innerItem)
{
return innerItem != null ? new OutputFieldItem(innerItem) : null;
}

public static implicit operator Item(OutputFieldItem customItem)
{
return customItem != null ? customItem.InnerItem : null;
}

public static bool IsValid(Item item)
{
  return item != null && item.IsDerivedFrom(TemplateId);
}

public bool IsValidOutputField()
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

public CustomLookupField InputField
{
get
{ 
return new CustomLookupField(InnerItem, InnerItem.Fields[InputFieldFieldName]);
}
}
public string InputFieldFieldName = "InputField";
public const string InputFieldConstFieldName = "InputField";


public CustomLookupField TemplateField
{
get
{ 
return new CustomLookupField(InnerItem, InnerItem.Fields[TemplateFieldFieldName]);
}
}
public string TemplateFieldFieldName = "TemplateField";
public const string TemplateFieldConstFieldName = "TemplateField";


#endregion //Field Instance Methods
}
}
