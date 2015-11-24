using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EzImporter
{
    public class Settings
    {
        public static ExistingItemHandling ExistingItemHandling
        {
            get
            {
                var value = Sitecore.Configuration.Settings.GetSetting("EzImporter.ExistingItemHandling", "AddVersion");
                ExistingItemHandling existingItemHandling;
                if (!Enum.TryParse<ExistingItemHandling>(value, out existingItemHandling))
                {
                    existingItemHandling = EzImporter.ExistingItemHandling.AddVersion;
                }
                return existingItemHandling;
            }
        }

        public static string MapsLocation
        {
            get
            {
                var value = Sitecore.Configuration.Settings.GetSetting("EzImporter.MapsLocation", "");
                return value;
            }
        }

        public static string RootItemQuery
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("EzImporter.RootItemQuery", "");
            }
        }

        public static InvalidLinkHandling InvalidLinkHandling
        {
            get
            {
                var value = Sitecore.Configuration.Settings.GetSetting("EzImporter.InvalidLinkHandling", "SetBroken");
                InvalidLinkHandling invalidLinkHandling;
                if (!Enum.TryParse<InvalidLinkHandling>(value, out invalidLinkHandling))
                {
                    invalidLinkHandling = EzImporter.InvalidLinkHandling.SetBroken;
                }
                return invalidLinkHandling;
            }
        }
    }
}