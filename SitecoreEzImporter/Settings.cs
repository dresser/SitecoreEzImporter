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
    }
}