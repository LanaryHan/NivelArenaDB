using System;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardFilterData : UIPanelData
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
    public class CardFilterUI : UIPanel
    {
        public FilterFlagGroup[] filterFlagGroups;
        public GameObject bg;
        public GameObject mask;        
        public RectTransform root;
        public Button button;

        private bool _isOpen;
        private Button _bgButton;
        
        public override bool CanCloseByBackKey => false;
        
        private Dictionary<string, Action<Enum,bool>> _callbacks = new();
        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            if (uiData is not CardFilterData)
            {
                return;
            }
            
            foreach (var filterFlagGroup in filterFlagGroups)
            {
                filterFlagGroup.Init(this);
            }

            _callbacks.Add("AttributeFlags", UpdateAttribute);
            _callbacks.Add("CardTypeFlags", UpdateCardType);
            _callbacks.Add("CostFlags", UpdateCost);
            _callbacks.Add("KeywordFlags", UpdateKeyword);

            var ec = GetEventComponent();
            ec.Listen<UIEvents.OnDialogOpen>(e =>
            {
                if (e.Dialog.dialogName is Dialog.Card_Details_UI)
                {
                    this.HideSelfByExt();
                }
            });
            ec.Listen<UIEvents.OnDialogClose>(e =>
            {
                if (e.Dialog is Dialog.Card_Details_UI)
                {
                    if (UIKit.GetPanel<CardsUI>())
                    {
                        this.ShowSelfByExt();
                    }
                }
            });
            
            _bgButton = bg.GetComponent<Button>();
            button.onClick.AddListener(OnClickFilter);
            _bgButton.onClick.AddListener(OnClickFilter);
        }

        private float GetDuration()
        {
            return !_isOpen
                ? root.anchoredPosition.x * -0.0015f + 0.375f
                : root.anchoredPosition.x * 0.0015f + 0.375f;
        }

        private void OnClickFilter()
        {
            root.DOKill();
            if (!_isOpen)
            {
                root.DOAnchorPosX(250f, GetDuration()).OnStart(() =>
                {
                    _isOpen = true;
                    bg.SetActive(true);
                    mask.SetActive(true);
                }).OnComplete(() =>
                {
                    mask.SetActive(false);
                });
            }
            else
            {
                root.DOAnchorPosX(-250f, GetDuration()).OnStart(() =>
                {
                    _isOpen = false;
                    bg.SetActive(false);
                    mask.SetActive(true);
                });
            }
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            bg.gameObject.SetActive(false);
            mask.gameObject.SetActive(true);
        }

        private void UpdateAttribute(Enum attributeFlags,bool value)
        {
            if (mUIData is not CardFilterData data)
            {
                return;
            }

            var flag = (AttributeFlags)attributeFlags;
            if (value)
            {
                data.AttributeFlags |= flag;
            }
            else
            {
                data.AttributeFlags &= ~flag;
            }

            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(mUIData as CardFilterData));
        }

        private void UpdateCost(Enum costFlag, bool value)
        {
            if (mUIData is not CardFilterData data)
            {
                return;
            }

            var flag = (CostFlags)costFlag;
            if (value)
            {
                data.CostFlags |= flag;
            }
            else
            {
                data.CostFlags &= ~flag;
            }
            
            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(mUIData as CardFilterData));
        }

        private void UpdateCardType(Enum cardTypeFlags, bool value)
        {
            if (mUIData is not CardFilterData data)
            {
                return;
            }
            
            var flag = (CardTypeFlags)cardTypeFlags;
            if (value)
            {
                data.CardTypeFlags |= flag;
            }
            else
            {
                data.CardTypeFlags &= ~flag;
            }
            
            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(mUIData as CardFilterData));
        }

        private void UpdateKeyword(Enum keywordFlags, bool value)
        {
            if (mUIData is not CardFilterData data)
            {
                return;
            }
            
            var flag = (KeywordFlags)keywordFlags;
            if (value)
            {
                data.KeywordFlags |= flag;
            }
            else
            {
                data.KeywordFlags &= ~flag;
            }
            
            GetEventComponent().Send(GameEvents.UpdateCardByFilter.Create(mUIData as CardFilterData));
        }
        public void UpdateFlags(string type, Enum flag, bool value)
        {
            _callbacks[type]?.Invoke(flag, value);
        }

        protected override void OnClose()
        {
            
        }
    }
}