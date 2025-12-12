namespace QFramework
{
    public class UIDefaultPanel : UIPanel
    {
        public override bool CanCloseByBackKey => false;

        protected override void OnInit(IUIData uiData = null)
        {
        }

        protected override void OnClose()
        {
        }
    }
}