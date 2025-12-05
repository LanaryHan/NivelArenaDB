using QFramework;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using UnityEngine;

namespace Runtime.Business.UI.Pack
{
    public class BoosterPackUI : UIPanel
    {
        [SerializeField] private PackButton tempBtn;
        [SerializeField] private Transform content;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempBtn.gameObject.SetActive(false);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            content.RemoveAllChildren(tempBtn.transform);

            foreach (var pack in DataManager.Instance.Packs.Keys)
            {
                var packButton = Instantiate(tempBtn, content);
                packButton.Init(pack);
                packButton.gameObject.SetActive(true);
            }
        }

        protected override void OnClose()
        {
            
        }
    }
}