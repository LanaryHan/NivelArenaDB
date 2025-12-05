using CsvHelper.Configuration;
using Runtime.Business.Data.Entry;

namespace Runtime.Business.Data.Map
{
    public class PackEntryMap : ClassMap<PackEntry>
    {
        public PackEntryMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.DisplayName).Index(1);
            Map(m => m.BundleRes).Index(2);
            Map(m => m.IsActive).Convert(row =>
            {
                var text = row.Row.GetField("IsActive");
                return !string.IsNullOrEmpty(text) && int.Parse(text) == 1;
            });
        }
    }
}