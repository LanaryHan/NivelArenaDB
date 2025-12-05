using DG.Tweening;
using GameEvents;
using QFramework;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace GameEvents
{
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
}

namespace Runtime.Business.Manager
{
    public class ExtUIManager : EventMonoBehaviour
    {
        protected ExtUIManager()
        {
        }

        public SpriteRenderer normalCard;
        public SpriteRenderer specialCard;
        public GameObject card;

        private bool _canReverse;
        private bool _reversed;
        private bool _isReversing;

        private void Start()
        {
            var ec = GetEventComponent();
            ec.Listen<ReverseCard>(ReverseCard);
            ec.Listen<ShowCard>(ShowCard);
            ec.Listen<HideCard>(HideCard);

            card.gameObject.SetActive(false);
        }

        private void HideCard(HideCard e)
        {
            card.SetActive(false);
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
    }
}