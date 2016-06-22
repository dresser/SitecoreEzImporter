using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace EzImporter.Commands
{
    public class LaunchEzImporter : Command
    {
        public override void Execute(CommandContext context)
        {
            string url = "/sitecore/client/Applications/EzImporter/EzImporterDialog";
            SheerResponse.ShowModalDialog(new ModalDialogOptions(url)
            {
                Width = "340px",
                Height = "400px",
                Response = false,
                ForceDialogSize = true,
                Maximizable = true
            });
        }
    }
}