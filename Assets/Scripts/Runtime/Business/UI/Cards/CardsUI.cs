using System.Linq;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using UnityEngine;

namespace UI
{
    public class CardsUIData : UIPanelData
    {
        public Deck Pack;

        public CardsUIData(Deck pack)
        {
            Pack = pack;
        }
    }

    public class CardsUI : UIPanel
    {
        [SerializeField] private CardGroup tempCardGroup;
        [SerializeField] private Transform content;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempCardGroup.gameObject.SetActive(false);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            if (uiData is not CardsUIData mainUIData)
            {
                return;
            }

            content.RemoveAllChildren(tempCardGroup.transform);
            var cards = DataManager.Instance.GetCardFromPack(mainUIData.Pack);
            var cardGroupBy = cards.OrderBy(card => card.CardType).GroupBy(card => card.CardType);
            using var enumerator = cardGroupBy.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var grouping = enumerator.Current;
                if (grouping != null)
                {
                    var cardEntries = grouping.ToList();
                    var cardGroup = Instantiate(tempCardGroup,content);
                    cardGroup.Init(cardEntries, grouping.Key.ToChinese());
                    cardGroup.gameObject.SetActive(true);
                }
            }
        }

        protected override void OnClose()
        {
            
        }
    }
}