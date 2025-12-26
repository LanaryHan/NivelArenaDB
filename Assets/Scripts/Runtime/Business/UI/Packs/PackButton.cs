using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PackButton : MonoBehaviour
    {
        public Image image;
        public TMP_Text packName;
        public Button button;

        private Deck _pack;
        public void Init(Deck pack)
        {
            button.onClick.RemoveAllListeners();
            _pack = pack;

            var packEntry = DataManager.Instance.GetPack(pack);
            var sprite = DataManager.Instance.LoadPackSprite(pack);
            image.sprite = sprite;
            packName.text = packEntry.DisplayName;
            button.onClick.AddListener(OnClick);
            gameObject.SetActive(packEntry.IsActive);
        }

        private void OnClick()
        {
            ExtUIManager.Instance.OpenDialog<CardsUI>(Dialog.Cards_UI, new CardsUIData(_pack));
            ExtUIManager.Instance.OpenDialog<CardFilterUI>(Dialog.Card_Filter_UI, new CardFilterData(), UILevel.PopUI);
        }
    }
}