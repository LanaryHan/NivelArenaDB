using QFramework;
using Runtime.Business.Manager;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class PresetDeck : EventMonoBehaviour
    {
        public Image cardImage;
        public TMP_Text nameText;
        public Button deleteBtn;
        public Button button;

        public void Init(string pdName, string leaderId)
        {
            nameText.text = pdName;
            var sprite = DataManager.Instance.LoadCardSprite(leaderId);
            cardImage.overrideSprite = sprite;
            deleteBtn.onClick.AddListener(() =>
            {
                EventManager.Instance.Send(GameEvents.DeleteDeck.Create(pdName));
            });
            button.onClick.AddListener(() =>
            {
                ExtUIManager.Instance.OpenDialog<DeckEditorUI>(Dialog.Deck_Editor_UI, new DeckEditorData(pdName));
            });
        }
    }
}