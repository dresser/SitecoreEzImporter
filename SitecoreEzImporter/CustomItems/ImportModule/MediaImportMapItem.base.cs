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
public partial class MediaImportMapItem : CustomItem
{

public static readonly string TemplateId = "{E90182E3-3CBE-43DA-B963-D93C3905B7BB}";


#region Boilerplate CustomItem Code

public MediaImportMapItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator MediaImportMapItem(Item innerItem)
{
return innerItem != null ? new MediaImportMapItem(innerItem) : null;
}

public static implicit operator Item(MediaImportMapItem customItem)
{
return customItem != null ? customItem.InnerItem : null;
}

public static bool IsValid(Item item)
{
  return item != null && item.IsDerivedFrom(TemplateId);
}

public bool IsValidMediaImportMap()
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


public CustomTextField InputFilenameFormat
{
get
{ 
return new CustomTextField(InnerItem, InnerItem.Fields[InputFilenameFormatFieldName]);
}
}
public string InputFilenameFormatFieldName = "InputFilenameFormat";
public const string InputFilenameFormatConstFieldName = "InputFilenameFormat";


public CustomTextField ItemIdProperty
{
get
{ 
return new CustomTextField(InnerItem, InnerItem.Fields[ItemIdPropertyFieldName]);
}
}
public string ItemIdPropertyFieldName = "ItemIdProperty";
public const string ItemIdPropertyConstFieldName = "ItemIdProperty";


public CustomTextField ImageFieldProperty
{
get
{ 
return new CustomTextField(InnerItem, InnerItem.Fields[ImageFieldPropertyFieldName]);
}
}
public string ImageFieldPropertyFieldName = "ImageFieldProperty";
public const string ImageFieldPropertyConstFieldName = "ImageFieldProperty";


public CustomCheckboxField UseFileNameForMediaItem
{
get
{ 
return new CustomCheckboxField(InnerItem, InnerItem.Fields[UseFileNameForMediaItemFieldName]);
}
}
public string UseFileNameForMediaItemFieldName = "UseFileNameForMediaItem";
public const string UseFileNameForMediaItemConstFieldName = "UseFileNameForMediaItem";


public CustomTextField NewMediaItemNameFormat
{
get
{ 
return new CustomTextField(InnerItem, InnerItem.Fields[NewMediaItemNameFormatFieldName]);
}
}
public string NewMediaItemNameFormatFieldName = "NewMediaItemNameFormat";
public const string NewMediaItemNameFormatConstFieldName = "NewMediaItemNameFormat";


public CustomTextField AltTextFormat
{
get
{ 
return new CustomTextField(InnerItem, InnerItem.Fields[AltTextFormatFieldName]);
}
}
public string AltTextFormatFieldName = "AltTextFormat";
public const string AltTextFormatConstFieldName = "AltTextFormat";


#endregion //Field Instance Methods
}
}
