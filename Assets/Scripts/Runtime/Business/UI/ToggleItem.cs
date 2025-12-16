using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.Business.UI
{
    public class ToggleItem : MonoBehaviour
    {
        public Toggle toggle;

        public UnityEvent<int> onShow;
        public UnityEvent<int> onHide;
    }
}