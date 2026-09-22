using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameEvents;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using ZEvent;
using Vector3 = UnityEngine.Vector3;

namespace UIEvents
{
    public class OnDialogShow : GameEventBaseNoDefaultCreate<OnDialogShow>
    {
        public UIBase Dialog;

        public static OnDialogShow Create(UIBase dialog)
        {
            var self = Create();
            self.Dialog = dialog;
            return self;
        }
    }
    
    public class OnDialogHide : GameEventBaseNoDefaultCreate<OnDialogHide>
    {
        public UIBase Dialog;

        public static OnDialogHide Create(UIBase dialog)
        {
            var self = Create();
            self.Dialog = dialog;
            return self;
        }
    }
    public class OnDialogOpen : GameEventBaseNoDefaultCreate<OnDialogOpen>
    {
        public UIBase Dialog;

        public static OnDialogOpen Create(UIBase dialog)
        {
            var self = Create();
            self.Dialog = dialog;
            return self;
        }
    }

    public class OnDialogClose : GameEventBaseNoDefaultCreate<OnDialogClose>
    {
        public UIBase Dialog;

        public static OnDialogClose Create(UIBase dialog)
        {
            var self = Create();
            self.Dialog = dialog;
            return self;
        }
    }
}

namespace GameEvents
{
    public class ClickBackKey : GameEventBase<ClickBackKey>{}
    public class ReverseCard : GameEventBase<ReverseCard>
    {
    }

    public class ShowCard : GameEventBaseNoDefaultCreate<ShowCard>
    {
        public string CardId;
        public bool ShowExtension;

        public static ShowCard Create(string cardId, bool showExtension)
        {
            var self = Create();
            self.CardId = cardId;
            self.ShowExtension = showExtension;
            return self;
        }
    }

    public class HideCard : GameEventBase<HideCard>
    {
        
    }

    public class CardFollowReady : GameEventBaseNoDefaultCreate<CardFollowReady>
    {
        public RectTransform Target;

        public static CardFollowReady Create(RectTransform target)
        {
            var self = Create();
            self.Target = target;
            return self;
        }
    }
}

namespace Runtime.Business.Manager
{
    public class ExtUIManager : MonoSingleton<ExtUIManager>
    {
        protected ExtUIManager()
        {
        }

        public Camera cardCamera;
        public Image normalCard;
        public Image specialCard;
        public GameObject card;

        private bool _canReverse;
        private bool _reversed;
        private bool _isReversing;

        protected override void Awake()
        {
#if UNITY_EDITOR
            cardCamera.orthographicSize = 10.15f;
#else
            cardCamera.orthographicSize = 12f;
#endif

            UIInit();
        }

        private void Start()
        {
            var ec = GetEventComponent();
            ec.Listen<ReverseCard>(ReverseCard);
            ec.Listen<ShowCard>(ShowCard);
            ec.Listen<HideCard>(HideCard);

            card.gameObject.SetActive(false);
        }
        
        private UniTask<GameObject> OnAssetRequest(Type type)
        {
            return ResManager.Instance.LoadAsync<GameObject>(type);
        }

        private void OnAssetRelease(Type type)
        {
            ResManager.Instance.Release(type);
        }

        private void UIInit()
        {
            UIFrame.OnAssetRequest += OnAssetRequest;
            UIFrame.OnAssetRelease += OnAssetRelease;

            UIFrame.OnCreate += OnCreateUI;
            UIFrame.OnShow += OnShowUI;
            UIFrame.OnHide += OnHideUI;
            UIFrame.OnDied += OnDiedUI;
        }

        private void OnCreateUI(UIBase uiBase)
        {
            EventManager.Instance.Send(UIEvents.OnDialogOpen.Create(uiBase));
        }

        private void OnShowUI(UIBase uiBase)
        {
            EventManager.Instance.Send(UIEvents.OnDialogShow.Create(uiBase));
        }

        private void OnHideUI(UIBase uiBase)
        {
            EventManager.Instance.Send(UIEvents.OnDialogHide.Create(uiBase));
        }

        private void OnDiedUI(UIBase uiBase)
        {
            EventManager.Instance.Send(UIEvents.OnDialogClose.Create(uiBase));
        }

        private void Release()
        {
            UIFrame.OnAssetRequest -= OnAssetRequest;
            UIFrame.OnAssetRelease -= OnAssetRelease;
            
            UIFrame.OnCreate -= OnCreateUI;
            UIFrame.OnShow -= OnShowUI;
            UIFrame.OnHide -= OnHideUI;
            UIFrame.OnDied -= OnDiedUI;
        }
        
        protected override void OnDestroy()
        {
            Release();
            base.OnDestroy();
        }

        #region Event

        private void HideCard(HideCard e)
        {
            card.SetActive(false);
            if (_isReversing)
            {
                card.transform.DOKill();
                _isReversing = false;
            }

            card.transform.rotation = Quaternion.identity;
            _reversed = false;
        }

        private void ShowCard(ShowCard evt)
        {
            var cardId = evt.CardId;
            var cardEntry = DataManager.Instance.GetCard(cardId);
            var normalSprite = ResManager.Instance.LoadCardSprite(cardId);
            normalCard.sprite = normalSprite;
            if (cardEntry.HasSpecial)
            {
                Sprite reverseSprite; 
                if (evt.ShowExtension)
                {
                    reverseSprite = ResManager.Instance.LoadExtensionCardSprite(cardId);
                    specialCard.overrideSprite = reverseSprite;
                }
                else
                {
                    reverseSprite = ResManager.Instance.LoadSpecialCardSprite(cardId);
                    specialCard.sprite = reverseSprite;
                    specialCard.overrideSprite = null;
                }

                _canReverse = true;
            }
            else
            {
                _canReverse = false;
            }
            
            card.gameObject.SetActive(true);
        }

        private void ReverseCard(ReverseCard evt)
        {
            if (!_canReverse)
            {
                return;
            }

            if (_isReversing)
            {
                return;
            }

            card.transform.DORotate(_reversed ? Vector3.zero : Vector3.up * 180f, 1.5f).OnStart(() =>
            {
                _isReversing = true;
            }).OnComplete(() =>
            {
                _reversed = !_reversed;
                _isReversing = false;
            });
        }

        #endregion
    }
}