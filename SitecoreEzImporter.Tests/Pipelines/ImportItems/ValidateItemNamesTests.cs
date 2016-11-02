using EzImporter.Map;
using EzImporter.Pipelines.ImportItems;
using Xunit;

namespace SitecoreEzImporter.Tests.Pipelines.ImportItems
{
    public class ValidateItemNamesTests
    {
        [Theory]
        [InlineData("GoodName")]
        [InlineData("Good Name")]
        public void ValidateItemNames_Success(string value)
        {
            var validateItemNamesProcessor = new ValidateItemNames();
            var importItem = new ItemDto(value); 
            validateItemNamesProcessor.ValidateName(importItem);
            Assert.Empty(validateItemNamesProcessor.Errors);
        }

        //[Theory]
        //[InlineData("'")]
        //[InlineData(@"\")]
        //[InlineData("/")]
        //[InlineData("?")]
        //[InlineData("£")]
        //[InlineData("#")]
        //public void ValidateItemNames_Failure(string value)
        //{
        //    var validateItemNamesProcessor = new ValidateItemNames();
        //    var importItem = new ImportItem(value);
        //    validateItemNamesProcessor.ValidateName(importItem);
        //    Assert.Single(validateItemNamesProcessor.Errors);
        //    Assert.Single(validateItemNamesProcessor.Notifications);
        //}
    }
}
