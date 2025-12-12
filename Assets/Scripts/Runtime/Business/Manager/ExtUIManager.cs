using System.Collections.Generic;
using DG.Tweening;
using GameEvents;
using QFramework;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace GameEvents
{
    public class ClickBackKey : GameEventBase<ClickBackKey>{}
    public class ReverseCard : GameEventBase<ReverseCard>
    {
    }

    public class ShowCard : GameEventBaseNoDefaultCreate<ShowCard>
    {
        public string CardId;

        public static ShowCard Create(string cardId)
        {
            var self = Create();
            self.CardId = cardId;
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
        public SpriteRenderer normalCard;
        public SpriteRenderer specialCard;
        public GameObject card;

        private bool _canReverse;
        private bool _reversed;
        private bool _isReversing;
        private readonly List<UIPanel> _uiList = new();

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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackKey();
            }
        }

        #region UI

        public void OpenDialog<T>() where T : UIPanel
        {
            var openPanel = UIKit.OpenPanel<T>();
            _uiList.Add(openPanel);
        }

        public void OpenDialog<T>(IUIData uiData) where T : UIPanel
        {
            var openPanel = UIKit.OpenPanel<T>(uiData);
            _uiList.Add(openPanel);
        }

        public void CloseDialog<T>(T dialog) where T : UIPanel
        {
            UIKit.ClosePanel(dialog);
            _uiList.Remove(dialog);
        }

        private void OnBackKey()
        {
            var uiPanel = _uiList[^1];
            if (uiPanel.CanCloseByBackKey)
            {
                _uiList.Remove(uiPanel);
                UIKit.ClosePanel(uiPanel);
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
            var normalSprite = DataManager.Instance.LoadCardSprite(cardId);
            normalCard.sprite = normalSprite;
            if (cardEntry.HasSpecial)
            {
                var specialSpite = DataManager.Instance.LoadSpecialCardSprite(cardId);
                specialCard.sprite = specialSpite;
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