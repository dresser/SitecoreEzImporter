using System;

namespace EzImporter.Configuration
{
    public class Factory
    {
        public static IImportOptions GetDefaultImportOptions()
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

            return new ImportOptions
            {
                ExistingItemHandling = existingItemHandling,
                InvalidLinkHandling = invalidLinkHandling,
                MultipleValuesImportSeparator =
                    Sitecore.Configuration.Settings.GetSetting("EzImporter.MultipleValuesImportSeparator", "|"),
                TreePathValuesImportSeparator =
                    Sitecore.Configuration.Settings.GetSetting("EzImporter.TreePathValuesImportSeparator", @"\"),
                CsvDelimiter = new[]
                {
                    Sitecore.Configuration.Settings.GetSetting("EzImporter.CsvDelimiter", ",")
                }
            };
        }
    }
}