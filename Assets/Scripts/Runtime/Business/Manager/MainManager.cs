using UI;
using UIFramework;
using UnityEngine.InputSystem;
using ZEvent;

namespace Runtime.Business.Manager
{
    public class MainManager : EventMonoBehaviour
    {
        private void Awake()
        {
            DataManager.Instance.InitCsv();
        }

        private void Start()
        {
            UIFrame.Show<PackUI>();
            UIFrame.Show<MenuUI>();
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                GetEventComponent().Send(GameEvents.ClickBackKey.Create());
            }
        }
    }
}