using System.Text;

namespace EzImporter.Pipelines.ImportItems
{
    public class ImportStatistics
    {
        public int InputDataRows { get; set; }
        public int CreatedItems { get; set; }
        public int UpdatedItems { get; set; }
        public StringBuilder Log { get; set; }

        public ImportStatistics()
        {
            InputDataRows = 0;
            CreatedItems = 0;
            UpdatedItems = 0;
            Log = new StringBuilder();
        }

        public override string ToString()
        {
            return string.Format("{0} rows read from input source.\r\n{1} items created.\r\n{2} items updated.",
                InputDataRows, CreatedItems, UpdatedItems);
        }
    }
}