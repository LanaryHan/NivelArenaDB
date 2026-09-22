using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Runtime.Business.UI;
using UIFramework;

namespace UI
{
    public class CardFilterData : UIData
    {
        public AttributeFlags AttributeFlags;
        public CardTypeFlags CardTypeFlags;
        public CostFlags CostFlags;
        public KeywordFlags KeywordFlags;

        public CardFilterData(AttributeFlags attributeFlags = AttributeFlags.None,
            CardTypeFlags cardTypeFlags = CardTypeFlags.None, 
            CostFlags costFlags = CostFlags.None,
            KeywordFlags keywordFlags = KeywordFlags.None)
        {
            AttributeFlags = attributeFlags;
            CardTypeFlags = cardTypeFlags;
            CostFlags = costFlags;
            KeywordFlags = keywordFlags;
        }
    }
    
    [WindowLayer]
    public class CardFilterUI : EdgeUIBase, IUIBaseData<CardFilterData>
    {
        public FilterFlagGroup[] filterFlagGroups;
        // public override bool CanCloseByBackKey => false;
        
        private Dictionary<string, Action<Enum,bool>> _callbacks = new();

        protected override UniTask OnCreate()
        {
            foreach (var filterFlagGroup in filterFlagGroups)
            {
                filterFlagGroup.Init(this);
            }

            _callbacks.Add("AttributeFlags", UpdateAttribute);
            _callbacks.Add("CardTypeFlags", UpdateCardType);
            _callbacks.Add("CostFlags", UpdateCost);
            _callbacks.Add("KeywordFlags", UpdateKeyword);
            
            var ec = GetEventComponent();
            ec.Listen<UIEvents.OnDialogShow>(e =>
            {
                if (e.Dialog is CardDetailUI)
                {
                    HideSelf();
                }
            });
            ec.Listen<UIEvents.OnDialogHide>(e =>
            {
                if (e.Dialog is CardDetailUI)
                {
                    if (UIFrame.Get<CardsUI>())
                    {
                        UIFrame.Show(this);
                    }
                }
            });

            OnShowEdgeStart += () =>
            {
                transform.SetAsLastSibling();
            };
            return base.OnCreate();
        }

        private void UpdateAttribute(Enum attributeFlags,bool value)
        {
            var flag = (AttributeFlags)attributeFlags;
            if (value)
            {
                Data.AttributeFlags |= flag;
            }
            else
            {
                Data.AttributeFlags &= ~flag;
            }

            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(Data));
        }

        private void UpdateCost(Enum costFlag, bool value)
        {
            
            var flag = (CostFlags)costFlag;
            if (value)
            {
                Data.CostFlags |= flag;
            }
            else
            {
                Data.CostFlags &= ~flag;
            }
            
            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(Data));
        }

        private void UpdateCardType(Enum cardTypeFlags, bool value)
        {
            var flag = (CardTypeFlags)cardTypeFlags;
            if (value)
            {
                Data.CardTypeFlags |= flag;
            }
            else
            {
                Data.CardTypeFlags &= ~flag;
            }
            
            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(Data));
        }

        private void UpdateKeyword(Enum keywordFlags, bool value)
        {
            var flag = (KeywordFlags)keywordFlags;
            if (value)
            {
                Data.KeywordFlags |= flag;
            }
            else
            {
                Data.KeywordFlags &= ~flag;
            }
            
            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(Data));
        }
        public void UpdateFlags(string type, Enum flag, bool value)
        {
            _callbacks[type]?.Invoke(flag, value);
        }
        
        public CardFilterData Data { get; set; }
    }
}