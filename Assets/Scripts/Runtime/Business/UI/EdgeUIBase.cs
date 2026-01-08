using System;
using Common;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Business.UI
{
    public class EdgeUIBase : UIPanel
    {
        public GameObject bg;
        public GameObject mask;
        public RectTransform root;
        public Button bgButton;
        public Button button;

        private bool _isOpen;
        private float _hideValue;
        private LinearMap _showMap, _hideMap;
        protected Action OnShowEdgeStart { get; set; }
        protected Action OnShowEdgeComplete { get; set; }
        protected Action OnHideEdgeStart { get; set; }
        protected Action OnHideEdgeComplete { get; set; }
        public override bool CanCloseByBackKey => false;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            _hideValue = root.anchoredPosition.x;
            _showMap = new LinearMap(-_hideValue, 0f, _hideValue, 0.75f);
            _hideMap = new LinearMap(_hideValue, 0f, -_hideValue, 0.75f);
            button.onClick.AddListener(OnClick);
            bgButton.onClick.AddListener(OnClick);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            bg.SetActive(false);
            mask.SetActive(true);
        }

        private float GetDuration()
        {
            return !_isOpen
                ? _showMap.Evaluate(root.anchoredPosition.x)
                : _hideMap.Evaluate(root.anchoredPosition.x);
        }
        
        private void OnClick()
        {
            root.DOKill();
            if (!_isOpen)
            {
                root.DOAnchorPosX(-_hideValue, GetDuration()).OnStart(() =>
                {
                    _isOpen = true;
                    bg.SetActive(true);
                    mask.SetActive(true);
                    OnShowEdgeStart?.Invoke();
                }).OnComplete(() =>
                {
                    mask.SetActive(false);
                    OnShowEdgeComplete?.Invoke();
                });
            }
            else
            {
                root.DOAnchorPosX(_hideValue, GetDuration()).OnStart(() =>
                {
                    _isOpen = false;
                    bg.SetActive(false);
                    mask.SetActive(true);
                    OnHideEdgeStart?.Invoke();
                }).OnComplete(() =>
                {
                    OnHideEdgeComplete?.Invoke();
                });
            }
        }

        public void DOQuick(bool open)
        {
            if (open)
            {
                root.DOAnchorPosX(-_hideValue, 0f);
                _isOpen = true;
                bg.SetActive(true);
                mask.SetActive(false);
            }
            else
            {
                root.DOAnchorPosX(_hideValue, 0f);
                _isOpen = false;
                bg.SetActive(false);
                mask.SetActive(true);
            }
        }

        protected override void OnClose()
        {
            
        }
    }
}