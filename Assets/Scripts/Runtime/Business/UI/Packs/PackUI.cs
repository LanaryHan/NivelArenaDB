using QFramework;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using UnityEngine;

namespace UI
{
    public class PackUI : UIPanel
    {
        public PackButton tempBtn;
        public Transform content;

        public override bool CanCloseByBackKey => false;

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
            }
        }

        protected override void OnClose()
        {
            
        }
    }
}