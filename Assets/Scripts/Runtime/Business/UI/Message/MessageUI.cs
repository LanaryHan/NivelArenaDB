using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Business.Manager;
using TMPro;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MessageParam : UIData
    {
        public Action<MessageUI.ButtonType, MessageUI> Callback { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public List<MessageUI.ButtonLabel> Buttons { get; private set; }
        public Action CloseCallback { get; private set; }
        public Action<MessageUI> StartCallback { get; private set; }

        public MessageParam SetTitle(string title)
        {
            Title = title;
            return this;
        }

        public MessageParam SetMessage(string message)
        {
            Message = message;
            return this;
        }

        public MessageParam SetOnClick(Action<MessageUI.ButtonType, MessageUI> callback)
        {
            Callback = callback;
            return this;
        }

        public MessageParam PositiveButton(string content)
        {
            Buttons ??= new List<MessageUI.ButtonLabel>();
            Buttons.Add(new MessageUI.ButtonLabel(MessageUI.ButtonType.PositiveBtn, content));
            return this;
        }

        public MessageParam NegativeButton(string content)
        {
            Buttons ??= new List<MessageUI.ButtonLabel>();
            Buttons.Add(new MessageUI.ButtonLabel(MessageUI.ButtonType.NegativeBtn, content));
            return this;
        }

        public MessageParam CloseButton()
        {
            Buttons ??= new List<MessageUI.ButtonLabel>();
            Buttons.Add(new MessageUI.ButtonLabel(MessageUI.ButtonType.CloseBtn, null));
            return this;
        }

        public MessageParam OnClose(Action callback)
        {
            CloseCallback = callback;
            return this;
        }

        public MessageParam OnStart(Action<MessageUI> callback)
        {
            StartCallback = callback;
            return this;
        }
    }
    
    [WindowLayer]
    public class MessageUI : UIComponent<MessageParam>
    {
        [Serializable]
        public class MessageButton
        {
            public Button btn;
            public TMP_Text label;
        }
        
        public enum ButtonType
        {
            PositiveBtn,
            NegativeBtn,
            CloseBtn
        }
        
        public struct ButtonLabel
        {
            public ButtonType Type;
            public string Content;

            public ButtonLabel(ButtonType type, string content)
            {
                Type = type;
                Content = content;
            }
        }

        public TMP_Text titleTxt;
        public Image image;
        public TMP_Text messageTxt;
        public MessageButton positiveBtn;
        public MessageButton negativeBtn;
        public MessageButton onlyPositiveBtn;
        public MessageButton onlyNegativeBtn;
        public Button closeBtn;

        private static string _imagePath;
        private static Vector2 _imageSize;
        // public override bool CanCloseByBackKey => false;

        public static MessageParam Create()
        {
            var param = new MessageParam();
            _imagePath = null;
            _imageSize = Vector2.zero;
            UIFrame.Show<MessageUI>(param);
            return param;
        }

        public static MessageParam CreateWithImage(string imageBundleName, Vector2 size)
        {
            var param = new MessageParam();
            _imagePath = imageBundleName;
            _imageSize = new Vector2(size.x, size.y);
            UIFrame.Show<MessageUI>(param);
            return param;
        }

        protected override void OnShow()
        {
            Data.StartCallback?.Invoke(this);
            base.OnShow();
        }

        private void Start()
        {
            InitImage();
            InitButtons();
            InitContent();
        }

        private void InitImage()
        {
            if (string.IsNullOrEmpty(_imagePath))
            {
                image.gameObject.SetActive(false);
                return;
            }

            var sprite = ResManager.Instance.Load<Sprite>(_imagePath);
            image.gameObject.SetActive(sprite);
            image.overrideSprite = sprite;
            if (_imageSize.x > ((RectTransform)transform).sizeDelta.x)
            {
                Debug.LogWarning("This size is bigger than the dialog");
            }

            image.rectTransform.sizeDelta = _imageSize;
        }

        private void InitButtons()
        {
            positiveBtn.btn.gameObject.SetActive(false);
            negativeBtn.btn.gameObject.SetActive(false);
            onlyPositiveBtn.btn.gameObject.SetActive(false);
            onlyNegativeBtn.btn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(false);

            if (Data.Buttons == null)
            {
                return;
            }

            var useSingle = Data.Buttons.Count(btn => btn.Type is ButtonType.NegativeBtn or ButtonType.PositiveBtn) == 1;
            var usePositive = useSingle ? onlyPositiveBtn : positiveBtn;
            var useNegative = useSingle ? onlyNegativeBtn : negativeBtn;
            foreach (var button in Data.Buttons)
            {
                if (button.Type is ButtonType.NegativeBtn)
                {
                    useNegative.btn.gameObject.SetActive(true);
                    useNegative.label.text = button.Content;
                    useNegative.btn.onClick.AddListener(() => OnButton(ButtonType.NegativeBtn));
                }
                else if (button.Type is ButtonType.PositiveBtn)
                {
                    usePositive.btn.gameObject.SetActive(true);
                    usePositive.label.text = button.Content;
                    usePositive.btn.onClick.AddListener(() => OnButton(ButtonType.PositiveBtn));
                }
                else
                {
                    closeBtn.gameObject.SetActive(true);
                    closeBtn.onClick.AddListener(() => OnButton(ButtonType.CloseBtn));
                }
            }
        }

        private void InitContent()
        {
            if (!string.IsNullOrEmpty(Data.Title))
            {
                titleTxt.enabled = true;
                titleTxt.text = Data.Title;
            }
            else
            {
                titleTxt.enabled = false;
            }

            if (!string.IsNullOrEmpty(Data.Message))
            {
                messageTxt.text = Data.Message;
            }
        }

        private void OnButton(ButtonType buttonType)
        {
            Data.Callback?.Invoke(buttonType, this);
        }
        
        protected override void OnDied()
        {
            Data.CloseCallback?.Invoke();
            _imagePath = null;
            _imageSize = Vector2.zero;
            base.OnDied();
        }
    }
}