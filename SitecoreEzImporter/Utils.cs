using Sitecore.Data.Items;
using System.Text.RegularExpressions;

namespace EzImporter
{
    public class Utils
    {
        public static string GetValidItemName(string proposedName)
        {
            var newName = proposedName;
            if (string.IsNullOrWhiteSpace(newName))
            {
                return UnNamedItem;
            }
            newName = ItemUtil.ProposeValidItemName(newName);
            newName = Regex.Replace(newName, @"\s+", " ");
            return newName;
        }

        public static string UnNamedItem
        {
            get { return "Unnamed item"; }
        }
    }
}