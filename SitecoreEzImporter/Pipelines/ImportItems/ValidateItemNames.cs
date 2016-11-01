using System.Linq;
using EzImporter.Map;
using System.Collections.Generic;

namespace EzImporter.Pipelines.ImportItems
{
    public class ValidateItemNames : ImportItemsProcessor
    {
        public List<string> Errors { get; protected set; }
        public List<string> Notifications { get; protected set; }

        public ValidateItemNames()
        {
            Errors = new List<string>();
            Notifications = new List<string>();
        }

        public override void Process(ImportItemsArgs args)
        {
            Errors = new List<string>();
            Notifications = new List<string>();
            foreach (var item in args.ImportItems)
            {
                ValidateName(item);
            }
            if (Errors.Any())
            {
                args.AddMessage("Invalid item name(s) in import data.");
                args.ErrorDetail = string.Join("\n\n", Errors);
                args.AbortPipeline();
            }
        }

        public void ValidateName(ItemDto item)
        {
            var suggestedName = Utils.GetValidItemName(item.Name);
            if (suggestedName == Utils.UnNamedItem)
            {
                Errors.Add(string.Format("Invalid item name '{0}'.", item.Name));
            }
            if (suggestedName != item.Name)
            {
                Notifications.Add(string.Format("Name '{0}' is not valid, using '{1}' instead.", item.Name,
                    suggestedName));
            }
            if (item.Children != null)
            {
                foreach (var child in item.Children)
                {
                    ValidateName(child);
                }
            }
        }
    }
}