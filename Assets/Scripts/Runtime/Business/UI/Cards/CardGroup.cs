using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using GameEvents;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardGroup : EnhancedScrollerCellView
    {
        public GridLayoutGroup layoutGroup;
        public GameObject line;
        public TMP_Text lineText;
        public Transform content;
        public CardButton tempBtn;

        private List<CardButton> _cards;
        public void Init(CardsUI.CardWrapperGroup groupInfo)
        {
            _cards = new List<CardButton>();
            tempBtn.gameObject.SetActive(false);
            content.RemoveAllChildren(tempBtn.transform, line.transform);
            if (string.IsNullOrEmpty(groupInfo.TypeName))
            {
                line.gameObject.SetActive(false);
                layoutGroup.padding.top = 50;
            }
            else
            {
                lineText.text = groupInfo.TypeName;
                layoutGroup.padding.top = 150;
                line.gameObject.SetActive(true);
            }

            foreach (var cardWrapper in groupInfo.Cards)
            {
                var cardButton = Instantiate(tempBtn,content);
                cardButton.Init(cardWrapper.CardEntry.Id);
                cardButton.gameObject.SetActive(true);
                _cards.Add(cardButton);
            }
        }

        public void UpdateView(UpdateCardByFilter e)
        {
            _cards.ForEach(card => card.gameObject.SetActive(card.UpdateView(e)));
            gameObject.SetActive(_cards.Any(card => card.gameObject.activeSelf));
        }

        public void Reset()
        {
            foreach (var card in _cards)
            {
                card.Reset();
            }
        }
    }
}