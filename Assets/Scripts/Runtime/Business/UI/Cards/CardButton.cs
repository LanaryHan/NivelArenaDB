using System.Collections.Generic;
using System.Linq;
using GameEvents;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
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
        private CardEntry _cardEntry;
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
            _cardEntry = cardEntry;
        }

        private void OnClick()
        {
            ExtUIManager.Instance.OpenDialog<CardDetailUI>(Dialog.Card_Details_UI, new CardDetailData(_cardId));
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

        public bool UpdateView(UpdateCardByFilter e)
        {
            if (e.CostFlags is not CostFlags.None)
            {
                if (_cardEntry.Cost == null)
                {
                    return false;
                }

                if ((e.CostFlags & CostFlags.TenPlus) != 0 && _cardEntry.Cost.Value >= 10)
                {
                    return true;
                }

                var flag = (CostFlags)(1 << _cardEntry.Cost.Value);
                if ((e.CostFlags & flag) == 0)
                {
                    return false;
                }
            }
            
            if (e.AttributeFlags is not AttributeFlags.None)
            {
                var flag = (AttributeFlags)(1 << (int)_cardEntry.Attribute);
                if ((e.AttributeFlags & flag) == 0)
                {
                    return false;
                }
            }

            if (e.CardTypeFlags is not CardTypeFlags.None)
            {
                var flag = (CardTypeFlags)(1 << (int)_cardEntry.CardType);
                if ((e.CardTypeFlags & flag) == 0)
                {
                    return false;
                }
            }

            var canShowKeywords = false;
            if (e.KeywordFlags is not KeywordFlags.None)
            {
                if (_cardEntry.Skills.Length == 0)
                {
                    return false;
                }

                var keys = new HashSet<KeyType>();
                foreach (var skillId in _cardEntry.Skills)
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
    }
}