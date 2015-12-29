using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EzImporter.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace EzImporter.Map.CustomItems
{
    public class MediaImportMapItem : CustomItem
    {
        public MediaImportMapItem(Item item) : base(item)
        {

        }

        public Item TargetTemplate
        {
            get { return ItemExtensions.GetLinkItem(base.InnerItem, "TargetTemplate"); }
        }

        public string InputFilenameFormat
        {
            get { return base.InnerItem["InputFilenameFormat"]; }
        }

        public string ItemIdProperty
        {
            get { return base.InnerItem["ItemIdProperty"]; }
        }

        public string ImageFieldProperty
        {
            get { return base.InnerItem["ImageFieldProperty"]; }
        }

        public bool UseFileNameForMediaItem
        {
            get { return base.InnerItem["UseFileNameForMediaItem"] == "1"; }
        }

        public string NewMediaItemNameFormat
        {
            get { return base.InnerItem["NewMediaItemNameFormat"]; }
        }

        public string AltTextFormat
        {
            get { return base.InnerItem["AltTextFormat"]; }
        }
    }
}