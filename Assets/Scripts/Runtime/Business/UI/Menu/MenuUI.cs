using Cysharp.Threading.Tasks;
using Logic;
using Runtime.Business.UI;
using UIFramework;
using UnityEngine.UI;

namespace UI
{
    [LoadingLayer]
    public class MenuUI : EdgeUIBase
    {
        public Button buildDeckBtn;
        // public override bool CanCloseByBackKey => false;

        protected override UniTask OnCreate()
        {
            OnShowEdgeStart += () =>
            {
                transform.SetAsLastSibling(); 
            };
            
            var ec = GetEventComponent();
            ec.Listen<UIEvents.OnDialogShow>(e =>
            {
                if (e.Dialog is MenuUI)
                {
                    return;
                }

                UpdateView();
            });
            ec.Listen<UIEvents.OnDialogHide>(e =>
            {
                if (e.Dialog is MenuUI)
                {
                    return;
                }

                UpdateView();
            });
            
            return base.OnCreate();
        }


        protected override void OnBind()
        {
            buildDeckBtn.onClick.AddListener(() =>
            {
                DOQuick(false);
                var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
                if (logic.IsBuilding)
                {
                    UIFrame.Show<DeckEditorUI>(new DeckEditorData(null, true));
                }
                else
                {
                    UIFrame.Show<PresetDeckUI>();
                }
            });
            base.OnBind();
        }

        protected override void OnUnbind()
        {
            buildDeckBtn.onClick.RemoveAllListeners();
            base.OnUnbind();
        }
        
        private void UpdateView()
        {
            if (UIFrame.GetLayerTransform<PanelLayer>().childCount == 1)
            {
                UIFrame.Show(this);
            }
            else
            {
                HideSelf();
            }
        }
    }
}