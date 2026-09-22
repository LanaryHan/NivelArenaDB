using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using GameEvents;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UI;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using ZEvent;

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
    public class CardsUIData : UIData
    {
        public Deck Pack;

        public CardsUIData(Deck pack)
        {
            Pack = pack;
        }
    }

    [PanelLayer]
    public class CardsUI : UIComponent<CardsUIData>, IEnhancedScrollerDelegate
    {
        public class CardWrapper
        {
            public CardEntry CardEntry;
            public bool Show;
        }
        public class CardWrapperGroup
        {
            public string TypeName;
            public List<CardWrapper> Cards = new();
        }
        
        public CardGroup tempCardGroup;
        public EnhancedScroller scroller;
        public Button closeBtn;
        public TMP_Text title;

        private readonly List<CardWrapper> _cardWrappers = new();
        private readonly List<CardWrapperGroup> _cardWrapperGroups = new();
        private float _cardItemSize;
        // public override bool CanCloseByBackKey => true;

        protected override UniTask OnCreate()
        {
            _cardItemSize = tempCardGroup.layoutGroup.cellSize.y;
            tempCardGroup.gameObject.SetActive(false);
            return base.OnCreate();
        }

        protected override void OnShow()
        {
            base.OnShow();
            InitData(Data.Pack);
            scroller.Delegate = this;
            scroller.cellViewWillRecycle += cell =>
            {
                var item = cell as CardGroup;
                item?.Reset();
            };
            scroller.ClearAll();
            scroller.ReloadData();
            
            var packEntry = DataManager.Instance.GetPack(Data.Pack);
            title.text = packEntry.Title;
        }

        protected override void OnBind()
        {
            closeBtn.onClick.AddListener(HideSelf);
            var ec = GetEventComponent();
            ec.Listen<UpdateCardByFilter>(OnUpdateCardByFilter);
            base.OnBind();
        }

        protected override void OnUnbind()
        {
            closeBtn.onClick.RemoveAllListeners();
            var ec = GetEventComponent();
            ec.ClearListeners();
            base.OnUnbind();
        }

        protected override void OnHide()
        {
            UIFrame.Hide<CardFilterUI>();
            base.OnHide();
        }

        private void InitData(Deck pack)
        {
            _cardWrappers.Clear();
            _cardWrapperGroups.Clear();
            var cards = DataManager.Instance.GetCardFromPack(pack);
            foreach (var wrapper in cards.Select(card => new CardWrapper
                     {
                         CardEntry = card,
                         Show = true,
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
        
        private void OnUpdateCardByFilter(UpdateCardByFilter e)
        {
            foreach (var wrapperGroup in _cardWrapperGroups)
            {
                foreach (var cardWrapper in wrapperGroup.Cards)
                {
                    var show = CheckCanShow(e, cardWrapper.CardEntry);
                    cardWrapper.Show = show;
                }
            }

            scroller.ReloadData();
        }
        
        private bool CheckCanShow(UpdateCardByFilter e, CardEntry cardEntry)
        {
            if (e.CostFlags is not CostFlags.None)
            {
                if (cardEntry.Cost == null)
                {
                    return false;
                }

                if ((e.CostFlags & CostFlags.TenPlus) != 0 && cardEntry.Cost.Value >= 10)
                {
                    return true;
                }

                var flag = (CostFlags)(1 << cardEntry.Cost.Value);
                if ((e.CostFlags & flag) == 0)
                {
                    return false;
                }
            }
            
            if (e.AttributeFlags is not AttributeFlags.None)
            {
                var flag = (AttributeFlags)(1 << (int)cardEntry.Attribute);
                if ((e.AttributeFlags & flag) == 0)
                {
                    return false;
                }
            }

            if (e.CardTypeFlags is not CardTypeFlags.None)
            {
                var flag = (CardTypeFlags)(1 << (int)cardEntry.CardType);
                if ((e.CardTypeFlags & flag) == 0)
                {
                    return false;
                }
            }

            if (e.KeywordFlags is not KeywordFlags.None)
            {
                if (cardEntry.Skills.Length == 0)
                {
                    return false;
                }

                var keys = new HashSet<KeyType>();
                foreach (var skillId in cardEntry.Skills)
                {
                    var skillEntry = DataManager.Instance.GetSkill(skillId);
                    if (skillEntry.Key1 is not KeyType.None)
                    {
                        keys.Add(skillEntry.Key1);
                    }

                    if (skillEntry.Key2 != null)
                    {
                        if (skillEntry.Key2.Value is not KeyType.None)
                        {
                            keys.Add(skillEntry.Key2.Value);
                        }
                    }
                }

                var flag = keys.Select(key => (KeywordFlags)(1 << (int)key - 1)).Aggregate(KeywordFlags.None, (current, f) => current | f);
                return (flag & e.KeywordFlags) != 0;
            }

            return true;
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
            size += string.IsNullOrEmpty(cardWrapperGroup.TypeName) ? 50 : 150;   //标题 + 空位
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
                    cellView.gameObject.SetActive(groupInfo.Cards.Any(wrapper => wrapper.Show));
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