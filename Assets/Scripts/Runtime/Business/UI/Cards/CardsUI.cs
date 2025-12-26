using System.Collections.Generic;
using System.Linq;
using GameEvents;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameEvents
{
    public class UpdateCardByFilter : GameEventBase<UpdateCardByFilter>
    {
        public AttributeFlags AttributeFlags;
        public CardTypeFlags CardTypeFlags;
        public KeywordFlags KeywordFlags;
        public CostFlags CostFlags;

        public static UpdateCardByFilter Create(CardFilterData cardFilterData)
        {
            var self = Create();
            self.AttributeFlags = cardFilterData.AttributeFlags;
            self.CardTypeFlags = cardFilterData.CardTypeFlags;
            self.KeywordFlags = cardFilterData.KeywordFlags;
            self.CostFlags = cardFilterData.CostFlags;
            return self;
        }
    }
}

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
        public CardGroup tempCardGroup;
        public Transform content;
        public Button closeBtn;
        public TMP_Text title;

        private List<CardGroup> _curGroups = new();
        public override bool CanCloseByBackKey => true;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempCardGroup.gameObject.SetActive(false);
            closeBtn.onClick.AddListener(this.CloseSelfByExt);
            var ec = GetEventComponent();
            ec.Listen<UpdateCardByFilter>(OnUpdateCardByFilter);
        }
        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            if (uiData is not CardsUIData mainUIData)
            {
                return;
            }

            _curGroups = new List<CardGroup>();
            content.RemoveAllChildren(tempCardGroup.transform, closeBtn.transform);
            var packEntry = DataManager.Instance.GetPack(mainUIData.Pack);
            title.text = packEntry.Title;
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
                    _curGroups.Add(cardGroup);
                }
            }
        }

        protected override void OnClose()
        {
            _curGroups = null;
            ExtUIManager.Instance.CloseDialog<CardFilterUI>();
        }
        
        private void OnUpdateCardByFilter(UpdateCardByFilter e)
        {
            _curGroups.ForEach(group => group.UpdateView(e));
        }
    }
}