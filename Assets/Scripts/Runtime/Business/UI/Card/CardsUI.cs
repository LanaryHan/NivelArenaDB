using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using UnityEngine;

namespace Runtime.Business.UI.Card
{
    public class CardsUIData : UIPanelData
    {
        public Deck Pack;
    }

    public class CardsUI : UIPanel
    {
        [SerializeField] private CardButton tempBtn;
        [SerializeField] private Transform content;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempBtn.gameObject.SetActive(false);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            if (uiData is not CardsUIData mainUIData)
            {
                return;
            }

            content.RemoveAllChildren(tempBtn.transform);
            var cards = DataManager.Instance.GetCardFromPack(mainUIData.Pack);
            foreach (var card in cards)
            {
                var cardButton = Instantiate(tempBtn, content);
                cardButton.Init(card.Id);
                cardButton.gameObject.SetActive(true);
            }
        }

        protected override void OnClose()
        {
            
        }
    }
}