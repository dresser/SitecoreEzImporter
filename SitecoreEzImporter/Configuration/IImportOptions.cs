namespace EzImporter.Configuration
{
    public interface IImportOptions
    {
        InvalidLinkHandling InvalidLinkHandling { get; set; }
        ExistingItemHandling ExistingItemHandling { get; set; }
        string MultipleValuesImportSeparator { get; set; }
        string TreePathValuesImportSeparator { get; set; }
    }
}
