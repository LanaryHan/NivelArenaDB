using System.Collections.Generic;
using System.Linq;
using GameEvents;
using Runtime.Business.Data.Entry;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardGroup : MonoBehaviour
    {
        public GridLayoutGroup layoutGroup;
        public GameObject line;
        public TMP_Text lineText;
        public Transform content;
        public CardButton tempBtn;

        private List<CardButton> _cards;
        public void Init(List<CardEntry> cardEntries, string showLineString)
        {
            _cards = new List<CardButton>();
            tempBtn.gameObject.SetActive(false);
            content.RemoveAllChildren(tempBtn.transform, line.transform);
            if (string.IsNullOrEmpty(showLineString))
            {
                line.gameObject.SetActive(false);
                layoutGroup.padding.top = 50;
            }
            else
            {
                lineText.text = showLineString;
                layoutGroup.padding.top = 150;
                line.gameObject.SetActive(true);
            }

            foreach (var cardEntry in cardEntries)
            {
                var cardButton = Instantiate(tempBtn,content);
                cardButton.Init(cardEntry.Id);
                cardButton.gameObject.SetActive(true);
                _cards.Add(cardButton);
            }
        }

        public void UpdateView(UpdateCardByFilter e)
        {
            _cards.ForEach(card => card.gameObject.SetActive(card.UpdateView(e)));
            gameObject.SetActive(_cards.Any(card => card.gameObject.activeSelf));
        }
    }
}