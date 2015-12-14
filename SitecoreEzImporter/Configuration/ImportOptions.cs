namespace EzImporter.Configuration
{
    public class ImportOptions : IImportOptions
    {
        public InvalidLinkHandling InvalidLinkHandling { get; set; }

        public ExistingItemHandling ExistingItemHandling { get; set; }

        public string MultipleValuesImportSeparator { get; set; }

        public string TreePathValuesImportSeparator { get; set; }
    }
}