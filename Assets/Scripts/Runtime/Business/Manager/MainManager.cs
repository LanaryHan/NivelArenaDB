using QFramework;
using Runtime.Business.UI;
using UnityEngine;

namespace Runtime.Business.Manager
{
    public class MainManager : MonoBehaviour
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
        }
    }
}