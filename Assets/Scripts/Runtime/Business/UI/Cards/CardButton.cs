using Runtime.Business.Data;
using Runtime.Business.Manager;
using UIFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Util;
using ZEvent;

namespace UI
{
    public class CardButton : EventMonoBehaviour
    {
        public Image image;
        public Button button;
        public Image frame;
        public Animation anim;
        public GameObject loading;

        private string _cardId;
        public async void Init(string cardId)
        {
            button.onClick.RemoveAllListeners();
            _cardId = cardId;

            loading.SetActive(true);
            anim.Play();
            var cardEntry = DataManager.Instance.GetCard(cardId);
            var spriteTask = ResManager.Instance.LoadCardSpriteAsync(cardId);
            var sprite = await spriteTask;
            if (image)
            {
                image.sprite = sprite;
                UpdateFrame(cardEntry.Attribute);
            }
            else
            {
                Addressables.Release(sprite);
            }

            button.onClick.AddListener(OnClick);
            anim.Stop();
            loading.SetActive(false);
        }

        private void OnClick()
        {
            UIFrame.Show<CardDetailUI>(new CardDetailData(_cardId));
        }
        private void UpdateFrame(ElementAttribute attribute)
        {
            frame.color = attribute switch
            {
                ElementAttribute.Flame => Color.red,
                ElementAttribute.Earth => "#019A75".ToRGB(),
                ElementAttribute.Storm => "#7555A0".ToRGB(),
                ElementAttribute.Wave => "#027FBD".ToRGB(),
                ElementAttribute.Lightning => Color.yellow,
                _ => Color.white
            };
        }
        
        public void Reset()
        {
            // _cardId = null;
        }
    }
}