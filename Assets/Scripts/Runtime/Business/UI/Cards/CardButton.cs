using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using Runtime.Business.UI.CardDetail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class CardButton : EventMonoBehaviour
    {
        public Image image;
        public TMP_Text cardName;
        public Button button;
        public Image[] attributeIcons;
        public Image frame;

        private string _cardId;
        public void Init(string cardId)
        {
            button.onClick.RemoveAllListeners();
            _cardId = cardId;
            
            var cardEntry = DataManager.Instance.GetCard(cardId);
            var sprite = DataManager.Instance.LoadCardSprite(cardId);
            image.sprite = sprite;
            cardName.text = cardEntry.Name;
            UpdateIcon(cardEntry.Attribute);
            UpdateFrame(cardEntry.Attribute);
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            UIKit.OpenPanel<CardDetailUI>(new CardDetailData(_cardId));
        }

        private void UpdateIcon(ElementAttribute attribute)
        {
            for (var i = 0; i < attributeIcons.Length; i++)
            {
                var attributeIcon = attributeIcons[i];
                attributeIcon.gameObject.SetActive(i == (int)attribute);
            }
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
    }
}