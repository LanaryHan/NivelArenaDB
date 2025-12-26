using QFramework;
using UI;
using UnityEngine;

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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GetEventComponent().Send(GameEvents.ClickBackKey.Create());
            }
        }
    }
}