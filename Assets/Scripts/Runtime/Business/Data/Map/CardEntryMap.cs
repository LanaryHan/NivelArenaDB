using System.Linq;
using CsvHelper.Configuration;
using Runtime.Business.Data.Entry;

namespace Runtime.Business.Data.Map
{
    public class CardEntryMap : ClassMap<CardEntry>
    {
        public CardEntryMap()
        {
            Map(m => m.Number).Index(0);
            Map(m => m.Pack).Index(1);
            Map(m => m.Rarity).Index(2);
            Map(m => m.CardType).Index(3);
            Map(m => m.Attribute).Index(4);
            Map(m => m.Name).Index(5);
            Map(m => m.Cost).Index(6);
            Map(m => m.Power).Index(7);
            Map(m => m.Hit).Index(8);
            Map(m => m.Skills).Convert(row =>
            {
                var text = row.Row.GetField("Skills");
                return string.IsNullOrEmpty(text)
                    ? null
                    : text.Split(",").Select(int.Parse).ToArray();
            });
            Map(m => m.Affiliation).Index(10);
            Map(m => m.Special).Index(11);
        }
    }
}