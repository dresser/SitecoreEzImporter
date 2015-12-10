using System;

namespace EzImporter
{
    public class Settings
    {
        public static Settings GetConfigurationSettings()
        {
            var value = Sitecore.Configuration.Settings.GetSetting("EzImporter.ExistingItemHandling", "AddVersion");
            ExistingItemHandling existingItemHandling;
            if (!Enum.TryParse<ExistingItemHandling>(value, out existingItemHandling))
            {
                existingItemHandling = EzImporter.ExistingItemHandling.AddVersion;
            }

            var invalidLinkHandlingValue = Sitecore.Configuration.Settings.GetSetting("EzImporter.InvalidLinkHandling",
                "SetBroken");
            InvalidLinkHandling invalidLinkHandling;
            if (!Enum.TryParse<InvalidLinkHandling>(invalidLinkHandlingValue, out invalidLinkHandling))
            {
                invalidLinkHandling = EzImporter.InvalidLinkHandling.SetBroken;
            }

            return new Settings
            {
                ExistingItemHandling = existingItemHandling,
                MapsLocation = Sitecore.Configuration.Settings.GetSetting("EzImporter.MapsLocation", ""),
                RootItemQuery = Sitecore.Configuration.Settings.GetSetting("EzImporter.RootItemQuery", ""),
                InvalidLinkHandling = invalidLinkHandling,
                MultipleValuesImportSeparator =
                    Sitecore.Configuration.Settings.GetSetting("EzImporter.MultipleValuesImportSeparator", "|"),
                ImportDirectory =
                    Sitecore.Configuration.Settings.GetSetting("EzImporter.ImportDirectory", "~/temp/EzImporter"),
                ImportItemsSubDirectory =
                    Sitecore.Configuration.Settings.GetSetting("EzImporter.ImportItemsSubDirectory", "Items"),
                ImportMediaSubDirectory =
                    Sitecore.Configuration.Settings.GetSetting("EzImporter.ImportMediaSubDirectory", "Items")
            };
        }

        public ExistingItemHandling ExistingItemHandling { get; set; }

        public string MapsLocation { get; set; }

        public string RootItemQuery { get; set; }

        public InvalidLinkHandling InvalidLinkHandling { get; set; }

        public string MultipleValuesImportSeparator { get; set; }

        public string ImportDirectory { get; set; }

        public string ImportItemsSubDirectory { get; set; }

        public string ImportMediaSubDirectory { get; set; }
    }
}