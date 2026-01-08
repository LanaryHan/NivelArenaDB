using System;
using System.Reflection;
using Runtime.Business.Util;
using UnityEngine;

namespace UI
{
    public class FilterFlagGroup : MonoBehaviour
    {
        public Transform topGroup;
        public Transform content;
        public FlagButton tempBtn;
        public string enumName;

        public void Init(CardFilterUI cardFilterUI)
        {
            tempBtn.gameObject.SetActive(false);
            content.RemoveAllChildren(tempBtn.transform, topGroup);
            
            var type = Assembly.GetExecutingAssembly().GetType($"UI.{enumName}");
            if (type == null)
            {
                return;
            }

            var values = Enum.GetValues(type);
            foreach (var value in values)
            {
                var i = Convert.ToInt32(value);
                if (i == 0)
                {
                    continue;
                }

                var @enum = value as Enum;
                var flagButton = Instantiate(tempBtn, content);
                flagButton.Init(@enum, cardFilterUI, enumName);
                flagButton.gameObject.SetActive(true);
            }
        }
    }
}