using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Manager;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class CardButton : EventMonoBehaviour
    {
        public Image image;
        public Button button;
        public Image frame;

        private string _cardId;
        private CardEntry _cardEntry;
        public void Init(string cardId)
        {
            button.onClick.RemoveAllListeners();
            _cardId = cardId;
            
            var cardEntry = DataManager.Instance.GetCard(cardId);
            var sprite = ResManager.Instance.LoadCardSprite(cardId);
            image.sprite = sprite;
            UpdateFrame(cardEntry.Attribute);
            button.onClick.AddListener(OnClick);
            _cardEntry = cardEntry;
        }

        private void OnClick()
        {
            ExtUIManager.Instance.OpenDialog<CardDetailUI>(Dialog.CardDetailUI, new CardDetailData(_cardId));
        }
        private void UpdateFrame(ElementAttribute attribute)
        {
            frame.color = attribute switch
            {
                ElementAttribute.Flame => Color.red,
                ElementAttribute.Earth => "#019A75".ToRGB(),
                ElementAttribute.Storm => "#7555A0".ToRGB(),
                ElementAttribute.Wave => "#027FBD".ToRGB(),
                ElementAttribute.Lightning => Color.yellow,
                _ => Color.white
            };
        }

        

        public void Reset()
        {
            _cardId = null;
            _cardEntry = null;
        }
    }
}