using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Business.UI
{
    public class ToggleGroupView : MonoBehaviour
    {
        public ToggleItem[] toggleItems;
    
        private readonly List<Toggle> _toggles = new();

        private void Awake()
        {
            var toggles = toggleItems.Select(item => item.toggle).ToList();
            for (var i = 0; i < toggles.Count; i++)
            {
                var index = i;
                toggles[i].onValueChanged.AddListener(call =>
                {
                    toggleItems[index].gameObject.SetActive(call);
                    if (call)
                    {
                        toggleItems[index]?.onShow?.Invoke(index);
                    }
                    else
                    {
                        toggleItems[index]?.onHide?.Invoke(index);
                    }
                });
            }

            _toggles.AddRange(toggles);
        }

        private void Start()
        {
            for (var i = 0; i < toggleItems.Length; i++)
            {
                var show = _toggles[i].isOn;
                toggleItems[i].gameObject.SetActive(show);
                if (show)
                {
                    toggleItems[i]?.onShow?.Invoke(i);
                }
                else
                {
                    toggleItems[i]?.onHide?.Invoke(i);
                }
            }
        }
    }
}