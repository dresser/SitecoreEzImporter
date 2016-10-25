using EzImporter.Pipelines.ImportItems;
using Xunit;

namespace SitecoreEzImporter.Tests.Pipelines.ImportItems
{
    public class ValidateArgsTests
    {
        [Fact]
        public void Process_Aborted()
        {
            var validateArgs = new ValidateArgs();
            var args = new ImportItemsArgs { FileStream = null };
            validateArgs.Process(args);
            Assert.True(args.Aborted);
        }
    }
}
