using System;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using UIFramework;
using UnityEngine;
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
        private string _acquiredCardId;
        private int _loadVersion;

        public async void Init(string cardId)
        {
            Reset();

            var version = _loadVersion;
            _cardId = cardId;

            loading.SetActive(true);
            anim.Play();
            var cardEntry = DataManager.Instance.GetCard(cardId);
            try
            {
                var sprite = await ResManager.Instance.LoadCardSpriteAsync(cardId);

                // Enhanced Scroller 可能已经回收并复用了这个 Cell。
                // 过期请求不能再更新 UI，但仍要释放本次加载取得的引用。
                if (!this || version != _loadVersion || _cardId != cardId || !image)
                {
                    ResManager.Instance.ReleaseCardSprite(cardId);
                    return;
                }

                _acquiredCardId = cardId;
                image.sprite = sprite;
                UpdateFrame(cardEntry.Attribute);

                button.onClick.AddListener(OnClick);
                anim.Stop();
                loading.SetActive(false);
            }
            catch (Exception exception)
            {
                // ResManager 会清理加载失败的缓存项，这里只处理当前 Cell 的状态。
                if (this && version == _loadVersion)
                {
                    anim.Stop();
                    loading.SetActive(false);
                    Debug.LogException(exception, this);
                }
            }
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
            // 使所有尚未完成的 Init 请求失效。
            _loadVersion++;

            button.onClick.RemoveAllListeners();
            if (image)
            {
                image.sprite = null;
            }

            if (!string.IsNullOrEmpty(_acquiredCardId))
            {
                ResManager.Instance.ReleaseCardSprite(_acquiredCardId);
                _acquiredCardId = null;
            }

            if (anim)
            {
                anim.Stop();
            }

            if (loading)
            {
                loading.SetActive(false);
            }

            _cardId = null;
        }

        private void OnDestroy()
        {
            Reset();
        }
    }
}
