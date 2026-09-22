using System;
using TMPro;
using UIFramework;
using UnityEngine.UI;

namespace UI
{
    public class NameInputParam : UIData
    {
        public string Title { get; private set; }
        public Action<string> Callback { get;private set; }


        public NameInputParam SetTitle(string title)
        {
            Title = title;
            return this;
        }

        public NameInputParam SetCallback(Action<string> callback)
        {
            Callback = callback;
            return this;
        }
    }
    
    [WindowLayer]
    public class InputFieldUI : UIComponent<NameInputParam>
    {
        public TMP_InputField nameInputField;
        public TMP_Text titleText;
        public Button closeBtn;
        public Button saveBtn;
        // public override bool CanCloseByBackKey => false;

        public static NameInputParam Create()
        {
            var param = new NameInputParam();
            UIFrame.Show<InputFieldUI>(param);
            return param;
        }

        private void Start()
        {
            titleText.text = Data.Title;
            closeBtn.onClick.AddListener(CloseSelf);
            saveBtn.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(nameInputField.text))
                {
                    return;
                }

                Data.Callback?.Invoke(nameInputField.text);
                CloseSelf();
            });
        }
    }
}