using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using Runtime.Business.UI.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Business.UI.Pack
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
        }

        private void OnClick()
        {
            UIKit.OpenPanel<CardsUI>(new CardsUIData
            {
                Pack = _pack,
            });
        }
    }
}