using QFramework;
using Runtime.Business.Manager;
using TMPro;
using UnityEngine.UI;

namespace Runtime.Business.UI.Card
{
    public class CardButton : EventMonoBehaviour
    {
        public Image image;
        public TMP_Text cardName;
        public Button button;

        private string _cardId;
        public void Init(string cardId)
        {
            button.onClick.RemoveAllListeners();
            _cardId = cardId;
            
            var cardEntry = DataManager.Instance.GetCard(cardId);
            var sprite = DataManager.Instance.LoadCardSprite(cardId);
            image.sprite = sprite;
            cardName.text = cardEntry.Name;
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GetEventComponent().Send(GameEvents.ClickCard.Create(_cardId));
        }
    }
}