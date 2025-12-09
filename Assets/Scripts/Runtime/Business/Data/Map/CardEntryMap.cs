using System.Collections.Generic;
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
                var skill1 = row.Row.GetField<int?>("Skill1");
                var skill2 = row.Row.GetField<int?>("Skill2");
                var skill3 = row.Row.GetField<int?>("Skill3");
                var skill4 = row.Row.GetField<int?>("Skill4");
                var list = new List<int?>
                {
                    skill1, skill2, skill3, skill4
                };
                list.RemoveAll(skill => skill == null);
                return list.Select(skill => skill!.Value).ToArray();
            });
            Map(m => m.SkillParams).Convert(row =>
            {
                var param1 = row.Row.GetField<string>("SkillParam1");
                var param2 = row.Row.GetField<string>("SkillParam2");
                var param3 = row.Row.GetField<string>("SkillParam3");
                var param4 = row.Row.GetField<string>("SkillParam4");
                var list = new List<string>
                {
                    param1, param2, param3, param4
                };
                return list.ToArray();
            });
            Map(m => m.IconTextParams).Convert(row =>
            {
                var param1 = row.Row.GetField<string>("IconTextParam1");
                var param2 = row.Row.GetField<string>("IconTextParam2");
                var param3 = row.Row.GetField<string>("IconTextParam3");
                var param4 = row.Row.GetField<string>("IconTextParam4");
                var list = new List<string>
                {
                    param1, param2, param3, param4
                };
                return list.ToArray();
            });
            Map(m => m.Trigger).Index(21);
            Map(m => m.TriggerParam).Index(22);
            Map(m => m.Affiliation).Index(23);
            Map(m => m.HasSpecial).Convert(row =>
            {
                var text = row.Row.GetField("HasSpecial");
                return !string.IsNullOrEmpty(text) && int.Parse(text) == 1;
            });
        }
    }
}