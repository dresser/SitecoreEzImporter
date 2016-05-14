namespace EzImporter.Models
{
    public class ImportResultModel
    {
        public string Log { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetail { get; set; }
    }
}