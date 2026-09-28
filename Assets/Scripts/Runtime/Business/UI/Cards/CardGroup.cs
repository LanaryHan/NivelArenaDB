using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using Runtime.Business.Util;
using UnityEngine;

namespace UI
{
    public class CardGroup : EnhancedScrollerCellView
    {
        public Transform content;
        public CardButton tempBtn;

        private List<CardButton> _cards;

        public void Init(CardsUI.CardWrapperGroup groupInfo)
        {
            _cards = new List<CardButton>();
            tempBtn.gameObject.SetActive(false);
            content.RemoveAllChildren(tempBtn.transform);

            foreach (var cardWrapper in groupInfo.Cards)
            {
                var cardButton = Instantiate(tempBtn, content);
                cardButton.Init(cardWrapper.CardEntry.Id);
                cardButton.gameObject.SetActive(cardWrapper.Show);
                _cards.Add(cardButton);
            }
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