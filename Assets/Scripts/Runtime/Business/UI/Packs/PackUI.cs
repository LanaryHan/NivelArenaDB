using QFramework;
using Runtime.Business.Manager;
using Runtime.Business.UI.CardDetail;
using Runtime.Business.UI.Message;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PackUI : UIPanel
    {
        [Header("Pack")]
        public PackButton tempBtn;
        public Transform[] contents;
        [Header("Search")]
        public TMP_InputField searchInput;
        public Button searchBtn;
        public Button clearBtn;
        

        public override bool CanCloseByBackKey => false;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempBtn.gameObject.SetActive(false);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            contents.ForEach(content => content.RemoveAllChildren());

            foreach (var (deck, entry) in DataManager.Instance.Packs)
            {
                var packButton = Instantiate(tempBtn, contents[entry.Group - 1]);
                packButton.Init(deck);
            }
            
            searchBtn.onClick.AddListener(OnClickSearch);
            clearBtn.onClick.AddListener(OnClickClear);
        }

        protected override void OnClose()
        {
            
        }
        
        private void OnClickClear()
        {
            searchInput.text = string.Empty;
        }

        private void OnClickSearch()
        {
            var id = searchInput.text;
            id = id.ToUpper();
            var messageParam = new MessageParam().SetTitle("Warning").PositiveButton("OK").CloseButton().SetOnClick(
                (_, ui) =>
                {
                    ExtUIManager.Instance.CloseDialog(ui);
                });
            if (id.Length != 7)
            {
                messageParam.SetMessage("卡牌id应该为7位数字字母组合，例如ST01001。");
                ExtUIManager.Instance.OpenDialog<MessageUI>(messageParam, UILevel.PopUI);
                return;
            }
            
            if (!id.StartsWith("ST") && !id.StartsWith("BT"))
            {
                messageParam.SetMessage("卡包应该以ST或BT开头，例如ST01001。");
                ExtUIManager.Instance.OpenDialog<MessageUI>(messageParam, UILevel.PopUI);
                return;
            }

            id = id.Insert(4, "-");
            Debug.Log(id);
            var cardEntry = DataManager.Instance.GetCard(id);
            if (cardEntry == null)
            {
                messageParam.SetMessage("卡牌id不存在！");
                ExtUIManager.Instance.OpenDialog<MessageUI>(messageParam, UILevel.PopUI);
                return;
            }

            ExtUIManager.Instance.OpenDialog<CardDetailUI>(new CardDetailData(cardEntry.Id));
        }
    }
}