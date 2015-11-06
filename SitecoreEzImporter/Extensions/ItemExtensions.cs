using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Linq;

namespace EzImporter.Extensions
{
    public static class ItemExtensions
    {
        public static bool HasVersion(this Item item)
        {
            return HasVersion(item, Sitecore.Context.Language);
        }

        public static bool HasVersion(this Item item, Language language)
        {
            Item itemInLanguage = Sitecore.Context.Database.GetItem(item.ID, language);
            return itemInLanguage.Versions.Count > 0;
        }

        public static bool IsDerivedFrom(this Item item, string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
                return false;

            Item templateItem = item.Database.Items[new ID(templateId)];
            if (templateItem == null)
                return false;

            //Template template = TemplateManager.GetTemplate(item);
            Template template = TemplateManager.GetTemplate(item.TemplateID, item.Database);
            if (template == null) return false;

            if (template.ID == templateItem.ID)
                return true;

            return template.DescendsFrom(templateItem.ID);
        }

        public static bool InheritsFrom(this Item item, ID templateId)
        {
            if (item == null)
            {
                return false;
            }

            Template template = TemplateManager.GetTemplate(item);
            Assert.IsNotNull(template, "Missing Template {0} in Database {1}", templateId, item.Database);

            return template.InheritsFrom(templateId);
        }

        /// <summary>
        /// True if the specified item is a descendant of an item of the specified template.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public static bool IsDescendantOfA(this Item item, ID templateId)
        {
            if (item == null)
            {
                return false;
            }
            foreach (var ancestor in item.Axes.GetAncestors())
            {
                if (ancestor.TemplateID == templateId)
                {
                    return true;
                }
            }
            return false;
        }

        public static Item FirstChildInheritingFrom(this Item item, ID templateId)
        {
            if (item == null)
            {
                return null;
            }
            return item.Children.FirstOrDefault(i => i.InheritsFrom(templateId));
        }

        public static Item FirstChildInheritingFrom(this Item item, string templateId)
        {
            if (templateId == null)
            {
                throw new ArgumentNullException("templateId");
            }
            ID id;
            if (!ID.TryParse(templateId, out id))
            {
                throw new ArgumentException("Invalid ID", "templateId");
            }
            return FirstChildInheritingFrom(item, id);
        }
    }
}