using QFramework;
using UI;
using UnityEngine.InputSystem;

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
            ExtUIManager.Instance.OpenDialog<PackUI>(Dialog.PackUI);
            ExtUIManager.Instance.OpenDialog<MenuUI>(Dialog.MenuUI, UILevel.PopUI);
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