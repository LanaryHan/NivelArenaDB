using Cysharp.Threading.Tasks;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [PanelLayer]
    public class PackUI : UIBase
    {
        [Header("Pack")]
        public PackButton tempBtn;
        public Transform[] contents;
        [Header("Search")]
        public TMP_InputField searchInput;
        public Button searchBtn;
        public Button clearBtn;
        

        // public override bool CanCloseByBackKey => false;
        
        protected override UniTask OnCreate()
        {
            tempBtn.gameObject.SetActive(false);
            return base.OnCreate();
        }

        protected override void OnShow()
        {
            foreach (var content in contents)
            {
                content.RemoveAllChildren();
            }

            foreach (var (deck, entry) in DataManager.Instance.Packs)
            {
                var packButton = Instantiate(tempBtn, contents[entry.Group - 1]);
                packButton.Init(deck);
            }
            
            searchBtn.onClick.AddListener(OnClickSearch);
            clearBtn.onClick.AddListener(OnClickClear);
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
                    UIFrame.Hide(ui);
                });
            if (id.Length != 7)
            {
                messageParam.SetMessage("卡牌id应该为7位数字字母组合，例如ST01001。");
                UIFrame.Show<MessageUI>(messageParam);
                return;
            }
            
            if (!id.StartsWith("ST") && !id.StartsWith("BT") && !id.StartsWith("SB"))
            {
                messageParam.SetMessage("卡包错误!");
                UIFrame.Show<MessageUI>(messageParam);
                return;
            }

            id = id.Insert(4, "-");
            var cardEntry = DataManager.Instance.TryGetCard(id);
            if (cardEntry == null)
            {
                messageParam.SetMessage("卡牌id不存在！");
                UIFrame.Show<MessageUI>(messageParam);
                return;
            }

            UIFrame.Show<CardDetailUI>(new CardDetailData(cardEntry.Id));
        }
    }
}