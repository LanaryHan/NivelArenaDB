using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using GameEvents;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
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

    public class CardsUI : UIPanel, IEnhancedScrollerDelegate
    {
        public class CardWrapper
        {
            public CardEntry CardEntry;
        }
        public class CardWrapperGroup
        {
            public string TypeName;
            public List<CardWrapper> Cards = new();
        }
        
        public CardGroup tempCardGroup;
        public EnhancedScroller scroller;
        public Transform content;
        public Button closeBtn;
        public TMP_Text title;

        private List<CardGroup> _curGroups = new();
        private readonly List<CardWrapper> _cardWrappers = new();
        private readonly List<CardWrapperGroup> _cardWrapperGroups = new();
        private float _cardItemSize;
        public override bool CanCloseByBackKey => true;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            if (uiData is not CardsUIData data)
            {
                return;
            }

            _cardItemSize = tempCardGroup.layoutGroup.cellSize.y;
            tempCardGroup.gameObject.SetActive(false);
            closeBtn.onClick.AddListener(this.CloseSelfByExt);

            InitData(data.Pack);
            scroller.Delegate = this;
            scroller.cellViewWillRecycle += cell =>
            {
                var item = cell as CardGroup;
                item?.Reset();
            };
            scroller.ClearAll();
            scroller.ReloadData();
            
            var ec = GetEventComponent();
            ec.Listen<UpdateCardByFilter>(OnUpdateCardByFilter);
        }

        private void InitData(Deck pack)
        {
            _cardWrappers.Clear();
            _cardWrapperGroups.Clear();
            var cards = DataManager.Instance.GetCardFromPack(pack);
            foreach (var wrapper in cards.Select(card => new CardWrapper
                     {
                         CardEntry = card,
                     }))
            {
                _cardWrappers.Add(wrapper);
            }

            var list = _cardWrappers.GroupBy(wrapper => wrapper.CardEntry.CardType);
            foreach (var group in list)
            {
                var cardWrappers = group.ToList();
                var cardWrapperGroup = new CardWrapperGroup()
                {
                    TypeName = group.Key.ToChinese(),
                    Cards = cardWrappers,
                };
                _cardWrapperGroups.Add(cardWrapperGroup);
            }
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
            /*var cards = DataManager.Instance.GetCardFromPack(mainUIData.Pack);
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
            }*/
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

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _cardWrapperGroups.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            var cardWrapperGroup = _cardWrapperGroups[dataIndex];
            var rawCount = Mathf.CeilToInt(cardWrapperGroup.Cards.Count / 3f);
            var size = _cardItemSize * rawCount + tempCardGroup.layoutGroup.spacing.y * (rawCount - 1);
            size += cardWrapperGroup.TypeName.IsNullOrEmpty() ? 50 : 150;   //标题 + 空位
            return size;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            EnhancedScrollerCellView cellView = null;
            if (dataIndex >= 0 && dataIndex < _cardWrapperGroups.Count)
            {
                var groupInfo = _cardWrapperGroups[dataIndex];
                cellView = scroller.GetCellView(tempCardGroup);
                if (cellView)
                {
                    cellView.gameObject.SetActive(true);
                    if (cellView is CardGroup cardGroup)
                    {
                        cardGroup.Init(groupInfo);
                    }
                }
            }
            
            return cellView;
        }
    }
}