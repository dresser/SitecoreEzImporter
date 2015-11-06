using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EzImporter
{
    public class Utils
    {
        public static string GetValidItemName(object proposedName)
        {
            return GetValidItemName(Convert.ToString(proposedName));
        }

        public static string GetValidItemName(string proposedName)
        {
            var newName = proposedName.Replace("-", " ");
            newName = ItemUtil.ProposeValidItemName(newName);
            newName = Regex.Replace(newName, @"\s+", " ");
            return newName;
        }
    }
}