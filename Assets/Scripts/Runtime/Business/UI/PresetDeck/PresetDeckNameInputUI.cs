using System;
using QFramework;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class NameInputParam : UIPanelData
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
    public class PresetDeckNameInputUI : UIPanel
    {
        public TMP_InputField nameInputField;
        public TMP_Text titleText;
        public Button closeBtn;
        public Button saveBtn;
        public override bool CanCloseByBackKey => false;

        public static NameInputParam Create()
        {
            var param = new NameInputParam();
            ExtUIManager.Instance.OpenDialog<PresetDeckNameInputUI>(Dialog.Preset_Deck_Name_Input_UI,UILevel.PopUI);
            return param;
        }

        private void Start()
        {
            if (mUIData is NameInputParam param)
            {
                titleText.text = param.Title;
                closeBtn.onClick.AddListener(this.CloseSelfByExt);
                saveBtn.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(nameInputField.text))
                    {
                        return;
                    }

                    param.Callback?.Invoke(nameInputField.text);
                });
            }
        }

        protected override void OnClose()
        {
            
        }
    }
}