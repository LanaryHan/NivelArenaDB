using Cysharp.Threading.Tasks;
using Logic;
using Runtime.Business.UI;
using UIFramework;
using UnityEngine.UI;
using ZEvent;

namespace UIEvents
{
    public class ShowMenu : GameEventBase<ShowMenu> { }
}

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
            return base.OnCreate();
        }


        protected override void OnBind()
        {
            var ec = GetEventComponent();
            ec.Listen<UIEvents.ShowMenu>(_ =>
            {
                Show();
            });
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
            var ec = GetEventComponent();
            ec.ClearListeners();
            buildDeckBtn.onClick.RemoveAllListeners();
            base.OnUnbind();
        }
    }
}