using System.Collections.Generic;
using CsvHelper.Configuration;
using Runtime.Business.Data.Entry;

namespace Runtime.Business.Data.Map
{
    public class KeywordExtensionEntryMap : ClassMap<KeywordExtensionEntry>
    {
        public KeywordExtensionEntryMap()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Keys).Convert(row =>
            {
                var key1 = row.Row.GetField<KeyType>("Key1");
                var key2 = row.Row.GetField<KeyType?>("Key2");
                var list = new List<KeyType>
                {
                    key1, key2 ?? KeyType.None
                };
                list.RemoveAll(key => key is KeyType.None);
                return list.ToArray();
            });
        }
    }
}