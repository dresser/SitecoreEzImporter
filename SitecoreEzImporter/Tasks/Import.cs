using Sitecore.Data.Items;
using Sitecore.Tasks;
using Sitecore.Diagnostics;

namespace EzImporter.Tasks
{
    public class Import
    {
        public void Run(Item[] items, CommandItem command, ScheduleItem schedule)
        {
            Log.Info("ExImporter.Tasks.Import.Run()" + command.ToString(), this);
        }
    }
}