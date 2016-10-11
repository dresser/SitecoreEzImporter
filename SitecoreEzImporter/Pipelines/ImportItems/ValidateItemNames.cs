using System.Linq;
using EzImporter.Map;
using System.Collections.Generic;

namespace EzImporter.Pipelines.ImportItems
{
    public class ValidateItemNames : ImportItemsProcessor
    {
        private List<string> _errors;
        private List<string> _notifications;

        public override void Process(ImportItemsArgs args)
        {
            _errors = new List<string>();
            _notifications = new List<string>();
            foreach (var item in args.ImportItems)
            {
                ValidateName(item);
            }
            if (_errors.Any())
            {
                _errors.ForEach(e => args.AddMessage(e));
                args.AbortPipeline();
            }
        }

        private void ValidateName(ImportItem item)
        {
            var suggestedName = Utils.GetValidItemName(item.Name);
            if (suggestedName == "")
            {
                _errors.Add(string.Format("Invalid item name '{0}'.", item.Name));
            }
            if (suggestedName != item.Name)
            {
                _notifications.Add(string.Format("Name '{0}' is not valid, using '{1}' instead.", item.Name,
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