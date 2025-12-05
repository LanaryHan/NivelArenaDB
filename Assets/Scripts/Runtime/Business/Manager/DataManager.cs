using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Data.Map;
using UnityEngine;

namespace Runtime.Business.Manager
{
    public class DataManager : Singleton<DataManager>
    {
        protected DataManager() { }
        
        public Dictionary<string, CardEntry> Cards;
        public Dictionary<Deck, PackEntry> Packs;
        public Dictionary<int, SkillEntry> Skills;

        private readonly ResLoader _resLoader = ResLoader.Allocate();

        public void InitCsv()
        {
            #region Card

            var csv = Resources.Load<TextAsset>("cards");
            using (var stringReader = new StringReader(csv.text))
            {
                using (var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Context.RegisterClassMap<CardEntryMap>();
                    var records = csvReader.GetRecords<CardEntry>();
                    Cards = records.ToDictionary(entry => entry.Id, entry => entry);
                }
            }

            #endregion

            #region Pack

            csv = Resources.Load<TextAsset>("packs");
            using (var stringReader = new StringReader(csv.text))
            {
                using (var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture))
                {
                    var records = csvReader.GetRecords<PackEntry>();
                    Packs = records.ToDictionary(entry => entry.Name, entry => entry);
                }
            }

            #endregion

            #region Skill

            csv = Resources.Load<TextAsset>("skills");
            using (var stringReader = new StringReader(csv.text))
            {
                using (var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture))
                {
                    var records = csvReader.GetRecords<SkillEntry>();
                    Skills = records.ToDictionary(entry => entry.Id, entry => entry);
                }
            }

            #endregion
        }

        public List<CardEntry> GetCardFromPack(Deck pack)
        {
            var cards = Cards.Values.ToList().Where(card => card.Pack == pack).ToList();
            return cards;
        }

        public Sprite LoadCardSprite(string res)
        {
            var sprite = _resLoader.LoadSync<Sprite>(res);
            return sprite;
        }

        public Sprite LoadPackSprite(Deck pack)
        {
            var packEntry = GetPack(pack);
            var sprite = _resLoader.LoadSync<Sprite>(packEntry.BundleRes);
            return sprite;
        }

        public CardEntry GetCard(string id)
        {
            return Cards.GetValueOrDefault(id);
        }

        public PackEntry GetPack(Deck pack)
        {
            return Packs.GetValueOrDefault(pack);
        }
    }
}