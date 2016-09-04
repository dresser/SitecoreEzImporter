using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace EzImporter.Tasks
{
    public class ImportCommandItem : CustomItem
    {
        public ImportCommandItem(Item item) : base(item)
        {

        }

        public string FileName
        {
            get { return InnerItem["FileName"]; }
        }

        public Database ImportDatabase
        {
            get
            {
                var database = Sitecore.Configuration.Factory.GetDatabase(InnerItem["Database"]);
                return database;
            }
        }

        public bool FirstRowAsColumnNames
        {
            get { return InnerItem["FirstRowAsColumnNames"] == "1"; }
        }

        public ID ImportLocationId
        {
            get
            {
                ID id;
                if (ID.TryParse(InnerItem["ImportLocation"], out id))
                {
                    return id;
                }
                return null;
            }
        }

        public Language TargetLanguage
        {
            get
            {
                ID id;
                if (!ID.TryParse(InnerItem["TargetLanguage"], out id))
                {
                    return null;
                }
                var langItem = Database.GetItem(id);
                if (langItem == null)
                {
                    return null;
                }
                return Language.Parse(langItem.Name);
            }
        }

        public ID ImportMapId
        {
            get
            {
                ID id;
                if (ID.TryParse(InnerItem["ImportMap"], out id))
                {
                    return id;
                }
                return null;
            }
        }

        public string CsvDelimiter
        {
            get { return InnerItem["CsvDelimiter"]; }
        }

        public string ExistingItemHandling
        {
            get { return GetDropDownItemValue(InnerItem, "ExistingItemHandling"); }
        }

        public string InvalidLinkHandling
        {
            get { return GetDropDownItemValue(InnerItem, "InvalidLinkHandling"); }
        }

        private static string GetDropDownItemValue(Item item, string fieldName)
        {
            ID id;
            if (ID.TryParse(item[fieldName], out id))
            {
                var dropDownItem = item.Database.GetItem(id);
                if (dropDownItem != null)
                {
                    return dropDownItem["Name"];
                }
            }
            return null;
        }

        public string MultipleValuesImportSeparator
        {
            get { return InnerItem["MultipleValuesImportSeparator"]; }
        }

        public string TreePathValuesImportSeparator
        {
            get { return InnerItem["TreePathValuesImportSeparator"]; }
        }
    }
}