using System;
using QFramework;
using Runtime.Business.Util;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class FlagButton : EventMonoBehaviour
    {
        private string _key;
        private Enum _enumFlag;
        private CardFilterUI _cardFilterUI;
        public TMP_Text showTxt;
        public Toggle toggle;

        public void Init(Enum enumFlag, CardFilterUI parent, string key)
        {
            _enumFlag = enumFlag;
            _cardFilterUI = parent;
            _key = key;
            showTxt.text = _enumFlag.ToChinese();
            toggle.onValueChanged.AddListener(value =>
            {
                parent.UpdateFlags(_key, _enumFlag, value); 
            });
        }
    }
}