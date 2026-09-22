using Runtime.Business.Manager;
using TMPro;
using UIFramework;
using UnityEngine.UI;
using ZEvent;

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
            var sprite = ResManager.Instance.LoadCardSprite(leaderId);
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
                UIFrame.Show<DeckEditorUI>(new DeckEditorData(pdName, !isPreset));
            });
        }
    }
}