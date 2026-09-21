using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameEvents;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace UIEvents
{
    public class OnDialogShow : GameEventBaseNoDefaultCreate<OnDialogShow>
    {
        public UIPanel Dialog;

        public static OnDialogShow Create(UIPanel dialog)
        {
            var self = Create();
            self.Dialog = dialog;
            return self;
        }
    }
    
    public class OnDialogHide : GameEventBaseNoDefaultCreate<OnDialogHide>
    {
        public UIPanel Dialog;

        public static OnDialogHide Create(UIPanel dialog)
        {
            var self = Create();
            self.Dialog = dialog;
            return self;
        }
    }
    public class OnDialogOpen : GameEventBaseNoDefaultCreate<OnDialogOpen>
    {
        public UIPanel Dialog;

        public static OnDialogOpen Create(UIPanel dialog)
        {
            var self = Create();
            self.Dialog = dialog;
            return self;
        }
    }

    public class OnDialogClose : GameEventBaseNoDefaultCreate<OnDialogClose>
    {
        public Dialog Dialog;

        public static OnDialogClose Create(Dialog dialog)
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

        private readonly Dictionary<UILevel, List<UIPanel>> _uiDic = new()
        {
            { UILevel.Common, new List<UIPanel>() },
            { UILevel.PopUI, new List<UIPanel>() }
        };

        private readonly Dictionary<Type, UILevel> _uiLevelMap = new();

        public Dictionary<UILevel, List<UIPanel>> UIDic => _uiDic;

        private void Awake()
        {
#if UNITY_EDITOR
            cardCamera.orthographicSize = 10.15f;
#else
            cardCamera.orthographicSize = 12f;
#endif
        }

        private void Start()
        {
            var ec = GetEventComponent();
            ec.Listen<ReverseCard>(ReverseCard);
            ec.Listen<ShowCard>(ShowCard);
            ec.Listen<HideCard>(HideCard);

            card.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                OnBackKey();
            }
        }

        #region UI

        private string GetDialogAddress(Dialog dialog)
        {
            return $"{dialog}/Dialog/{dialog}.prefab";
        }
        public T OpenDialog<T>(Dialog dialogName, UILevel uiLevel = UILevel.Common) where T : UIPanel
        {
            var path = GetDialogAddress(dialogName);
            var panelObj = ResManager.Instance.LoadPrefabAndInstantiate<GameObject>(path);
            var uiPanel = panelObj.GetComponent<UIPanel>();
            UIKit.Root.SetLevelOfPanel(uiLevel, uiPanel);
            UIKit.Config.SetDefaultSizeOfPanel(uiPanel);
            panelObj.name = dialogName.ToString();
            uiPanel.dialogName = dialogName;
            uiPanel.Info = PanelInfo.Allocate(dialogName.ToString(), uiLevel, null, typeof(T), null);
            UIKit.Table.Add(uiPanel);
            uiPanel.Init();
            uiPanel.Open();
            uiPanel.Show();
            _uiDic[uiLevel].Add(uiPanel);
            _uiLevelMap.TryAdd(typeof(T), uiLevel);
            GetEventComponent().Send(UIEvents.OnDialogOpen.Create(uiPanel));
            return uiPanel as T;
        }
        
        public T OpenDialog<T>(Dialog dialog, IUIData uiData, UILevel uiLevel = UILevel.Common) where T : UIPanel
        {
            var path = GetDialogAddress(dialog);
            var panelObj = ResManager.Instance.LoadPrefabAndInstantiate<GameObject>(path);
            var uiPanel = panelObj.GetComponent<UIPanel>();
            UIKit.Root.SetLevelOfPanel(uiLevel, uiPanel);
            UIKit.Config.SetDefaultSizeOfPanel(uiPanel);
            panelObj.name = dialog.ToString();
            uiPanel.dialogName = dialog;
            uiPanel.Info = PanelInfo.Allocate(dialog.ToString(), uiLevel, uiData, typeof(T), null);
            UIKit.Table.Add(uiPanel);
            uiPanel.Init(uiData);
            uiPanel.Open(uiData);
            uiPanel.Show();
            _uiDic[uiLevel].Add(uiPanel);
            _uiLevelMap.TryAdd(typeof(T), uiLevel);
            GetEventComponent().Send(UIEvents.OnDialogOpen.Create(uiPanel));
            return uiPanel as T;
        }
        
        public void CloseDialog<T>() where T : UIPanel
        {
            var uiLevel = _uiLevelMap[typeof(T)];
            var dialog = _uiDic[uiLevel].Find(ui => ui is T);
            var dialogName = dialog.dialogName;
            var searchKey = PanelSearchKeys.Allocate();
            searchKey.PanelType = typeof(T);
            searchKey.Level = uiLevel;
            var iPanel = UIKit.Table.GetPanelsByPanelSearchKeys(searchKey).LastOrDefault();
            if (iPanel is UIPanel)
            {
                iPanel.Close(fromLoad: false);
                UIKit.Table.Remove(iPanel);
                iPanel.Info.Recycle2Cache();
                iPanel.Info = null;
                searchKey.Recycle2Cache();
            }

            _uiDic[uiLevel].Remove(dialog);
            GetEventComponent().Send(UIEvents.OnDialogClose.Create(dialogName));
        }

        public void CloseDialog<T>(T dialog) where T : UIPanel
        {
            var uiLevel = _uiLevelMap[dialog.GetType()];
            var dialogName = dialog.dialogName;
            var searchKey = PanelSearchKeys.Allocate();
            searchKey.PanelType = typeof(T);
            searchKey.Level = uiLevel;
            var iPanel = UIKit.Table.GetPanelsByPanelSearchKeys(searchKey).LastOrDefault();
            if (iPanel is UIPanel)
            {
                iPanel.Close(fromLoad: false);
                UIKit.Table.Remove(iPanel);
                iPanel.Info.Recycle2Cache();
                iPanel.Info = null;
                searchKey.Recycle2Cache();
            }

            _uiDic[uiLevel].Remove(dialog);
            GetEventComponent().Send(UIEvents.OnDialogClose.Create(dialogName));
        }

        public void HideDialog<T>() where T : UIPanel
        {
            var uiLevel = _uiLevelMap[typeof(T)];
            var dialog = _uiDic[uiLevel].Find(ui => ui is T);
            dialog.Hide();
            dialog.gameObject.SetActive(false);
            GetEventComponent().Send(UIEvents.OnDialogHide.Create(dialog));
        }

        public void HideDialog<T>(T dialog) where T : UIPanel
        {
            dialog.Hide();
            dialog.gameObject.SetActive(false);
            GetEventComponent().Send(UIEvents.OnDialogHide.Create(dialog));
        }

        public void ShowDialog<T>() where T : UIPanel
        {
            var uiLevel = _uiLevelMap[typeof(T)];
            var dialog = _uiDic[uiLevel].Find(ui => ui is T);
            dialog.Show();
            dialog.gameObject.SetActive(true);
            GetEventComponent().Send(UIEvents.OnDialogShow.Create(dialog));
        }

        public void ShowDialog<T>(T dialog) where T : UIPanel
        {
            dialog.Show();
            dialog.gameObject.SetActive(true);
            GetEventComponent().Send(UIEvents.OnDialogShow.Create(dialog));
        }

        private void OnBackKey()
        {
            if (_uiDic[UILevel.PopUI].Count != 0)
            {
                var popPanel = _uiDic[UILevel.PopUI].LastOrDefault();
                if (popPanel && popPanel.CanCloseByBackKey)
                {
                    CloseDialog(popPanel);
                    return;
                }
            }

            var uiPanel = _uiDic[UILevel.Common].LastOrDefault();
            if (uiPanel && uiPanel.CanCloseByBackKey)
            {
                CloseDialog(uiPanel);
            }
        }

        #endregion

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