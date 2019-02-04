using System.Linq;
using System.Data;
using Xunit;
using FluentAssertions;

namespace SitecoreEzImporter.Tests.Extensions
{
    public class DataTableExtensionsTests
    {
        public DataTable GetSampleData()
        {
            var dt = new DataTable();
            dt.Columns.Add("ProductType", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("ID", typeof(string));
            dt.Rows.Add("Apples", "10", "£30", "01233");
            dt.Rows.Add("Apples", "20", "£50", "01234");
            dt.Rows.Add("Oranges", "1", "£1", "01235");
            dt.Rows.Add("Oranges", "2", "£2", "01236");
            return dt;
        }

        [Fact]
        public void GroupBy_Success()
        {
            var dataTable = GetSampleData();
            var groupedDataTable = EzImporter.Extensions.DataTableExtensions.GroupBy(dataTable, new[] { "ProductType" });

            groupedDataTable.Columns.Should().HaveCount(1);

            groupedDataTable.Rows.Should().HaveCount(2);
        }
    }
}
