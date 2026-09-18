using QFramework;
using UI;
using UnityEngine.InputSystem;

namespace Runtime.Business.Manager
{
    public class MainManager : EventMonoBehaviour
    {
        private ResLoader _resLoader;
        private void Awake()
        {
            ResKit.Init();
            DataManager.Instance.InitCsv();
        }

        private void Start()
        {
            _resLoader = ResLoader.Allocate();
            ExtUIManager.Instance.OpenDialog<PackUI>(Dialog.Pack_UI);
            ExtUIManager.Instance.OpenDialog<MenuUI>(Dialog.Menu_UI, UILevel.PopUI);
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