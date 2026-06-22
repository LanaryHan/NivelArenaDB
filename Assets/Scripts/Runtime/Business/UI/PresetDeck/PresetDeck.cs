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
        public Button editBtn;
        public Button button;

        private string _name;

        public void Init(string pdName, string leaderId,bool isPreset)
        {
            _name = pdName;
            nameText.text = pdName;
            var sprite = DataManager.Instance.LoadCardSprite(leaderId);
            cardImage.sprite = sprite;
            deleteBtn.gameObject.SetActive(!isPreset);
            editBtn.gameObject.SetActive(!isPreset);
            if (!isPreset)
            {
                deleteBtn.onClick.AddListener(() =>
                {
                    EventManager.Instance.Send(GameEvents.DeleteDeck.Create(pdName));
                });
                editBtn.onClick.AddListener(() =>
                {
                    InputFieldUI.Create().SetTitle("重新设置名称").SetCallback(text =>
                    {
                        GetEventComponent().Send(GameEvents.RenameDeck.Create(_name, text));
                    });
                });
            }

            button.onClick.AddListener(() =>
            {
                ExtUIManager.Instance.OpenDialog<DeckEditorUI>(Dialog.Deck_Editor_UI,
                    new DeckEditorData(pdName, !isPreset));
            });
        }
    }
}