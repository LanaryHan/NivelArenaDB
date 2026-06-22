using Logic;
using QFramework;
using Runtime.Business.Manager;
using Runtime.Business.UI;
using Runtime.Business.Util;
using UnityEngine.UI;

namespace UI
{
    public class MenuUI : EdgeUIBase
    {
        public Button buildDeckBtn;
        public override bool CanCloseByBackKey => false;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            var ec = GetEventComponent();
            ec.Listen<UIEvents.OnDialogOpen>(_ =>
            {
                UpdateView();
            });
            ec.Listen<UIEvents.OnDialogClose>(_ =>
            {
                UpdateView();
            });
            buildDeckBtn.onClick.AddListener(() =>
            {
                DOQuick(false);
                var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
                if (logic.IsBuilding)
                {
                    ExtUIManager.Instance.OpenDialog<DeckEditorUI>(Dialog.Deck_Editor_UI,
                        new DeckEditorData(null, true));
                }
                else
                {
                    ExtUIManager.Instance.OpenDialog<PresetDeckUI>(Dialog.Preset_Deck_UI);
                }
            });
            OnShowEdgeStart += () =>
            {
                transform.SetAsLastSibling(); 
            };
        }

        private void UpdateView()
        {
            if (ExtUIManager.Instance.UIDic[UILevel.Common].Count == 1)
            {
                this.ShowSelfByExt();
            }
            else
            {
                this.HideSelfByExt();
            }
        }
        protected override void OnClose()
        {
            
        }
    }
}