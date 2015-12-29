using EzImporter.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace EzImporter.Map.CustomItems
{
    public class OutputMapTemplateItem : CustomItem
    {
        public static readonly ID TemplateId = new ID("{58623B95-07DC-4F00-8D7A-337BF116FB54}");

        public OutputMapTemplateItem(Item item) : base(item)
        {

        }

        public Item TargetTemplate
        {
            get { return ItemExtensions.GetLinkItem(this.InnerItem, "TargetTemplate"); }
        }

        public Item ItemNameField
        {
            get { return ItemExtensions.GetLinkItem(this.InnerItem, "ItemNameField"); }
        }
    }
}