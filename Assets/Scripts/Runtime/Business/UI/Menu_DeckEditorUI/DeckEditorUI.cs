using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Logic;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UIEvents;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using ZEvent;

namespace UI
{
    public class DeckEditorData : UIData
    {
        public string DeckName;
        public bool CanEdit;

        public DeckEditorData(string deckName, bool canEdit)
        {
            DeckName = deckName;
            CanEdit = canEdit;
        }
    }
    
    [PanelLayer]
    public class DeckEditorUI : UIComponent<DeckEditorData>
    {
        public Button closeBtn;
        public Button leaderCard;
        public Button tempCard;
        public Image leaderImage;
        public Transform content1;
        public Transform content2;
        public TMP_Text nameTxt;
        public Button saveBtn;
        public Button cancelBtn;
        public Button editBtn;

        private string _deckName;
        // public override bool CanCloseByBackKey => true;
        private string _leaderId;

        protected override UniTask OnCreate()
        {
            tempCard.gameObject.SetActive(false);
            saveBtn.gameObject.SetActive(Data.CanEdit);
            editBtn.gameObject.SetActive(Data.CanEdit);
            cancelBtn.gameObject.SetActive(Data.CanEdit);
            return base.OnCreate();
        }

        protected override void OnShow()
        {
            var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
            if (logic.IsBuilding)
            {
                UpdateViewBuilding();
            }
            else
            {
                UpdateView(logic.GetDeckEntry(Data.DeckName));
            }

            if (Data.CanEdit)
            {
                UpdateButtons(logic.IsBuilding);
            }
            base.OnShow();
        }

        protected override void OnBind()
        {
            var ec = GetEventComponent();
            ec.Listen<OnDialogClose>(evt =>
            {
                if (evt.Dialog is CardDetailUI)
                {
                    var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
                    if (logic.IsBuilding)
                    {
                        content1.RemoveAllChildren();
                        content2.RemoveAllChildren();
                        UpdateViewBuilding();
                    }
                }
            });
            closeBtn.onClick.AddListener(CloseSelf);
            if (Data.CanEdit)
            {
                saveBtn.onClick.AddListener(() =>
                {
                    GetEventComponent().Send(GameEvents.SaveDeck.Create());
                    UpdateButtons(false);
                });
                editBtn.onClick.AddListener(() =>
                {
                    GetEventComponent().Send(GameEvents.EditDeck.Create(_deckName));
                    UpdateButtons(true);
                });
            }
            cancelBtn.onClick.AddListener(OnClickCancel);
            leaderCard.onClick.AddListener(OnClickLeaderCard);
            base.OnBind();
        }

        protected override void OnUnbind()
        {
            var ec = GetEventComponent();
            ec.ClearListeners();
            closeBtn.onClick.RemoveAllListeners();
            saveBtn.onClick.RemoveAllListeners();
            editBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();
            leaderCard.onClick.RemoveAllListeners();
            base.OnUnbind();
        }
        
        private void OnClickCancel()
        {
            if (string.IsNullOrEmpty(_deckName))
            {
                MessageUI.Create().SetTitle("提示")
                    .SetMessage("是否移除尚未保存的卡组?")
                    .PositiveButton("取消")
                    .NegativeButton("确认")
                    .SetOnClick((b, u) =>
                    {
                        if (b is MessageUI.ButtonType.NegativeBtn)
                        {
                            EventManager.Instance.Send(GameEvents.SetBuildingState.Create(false, CloseSelf));
                        }

                        UIFrame.Hide(u);
                    });
            }
            else
            {
                GetEventComponent().Send(GameEvents.SetBuildingState.Create(false, () =>
                {
                    var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
                    UpdateView(logic.GetDeckEntry(_deckName));
                }));
            }
        }

        private void UpdateViewBuilding()
        {
            var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
            var cardIds = logic.CardIds;
            UpdateCards(new List<string>(cardIds) { logic.LeaderCard.Id });
        }

        private void UpdateView(BuildDeckEntry entry)
        {
            var cardIds = entry.CardIds;
            nameTxt.text = entry.DeckName;
            _deckName = entry.DeckName;
            UpdateCards(cardIds);
        }

        private void UpdateCards(List<string> cardIds)
        {
            var cardEntries = cardIds.Select(id => DataManager.Instance.GetCard(id)).ToList();
            var leaderCard = cardEntries.FirstOrDefault(card => card.CardType is CardType.Leader);
            if (leaderCard != null)
            {
                var sprite = ResManager.Instance.LoadCardSprite(leaderCard.Id);
                leaderImage.overrideSprite = sprite;
                cardEntries.Remove(leaderCard);
            }

            _leaderId = leaderCard?.Id ?? string.Empty;
            cardEntries = (from card in cardEntries
                orderby card.CardType, card.Cost, card.Id
                select card).ToList();
            for (int i = 0; i < cardEntries.Count; i++)
            {
                var cardEntry = cardEntries[i];
                var btn = Instantiate(tempCard, i < 4 ? content1 : content2);
                var cardSprite = ResManager.Instance.LoadCardSprite(cardEntry.Id);
                btn.image.sprite = cardSprite;
                btn.gameObject.SetActive(true);
                btn.onClick.AddListener(() =>
                {
                    UIFrame.Show<CardDetailUI>(new CardDetailData(cardEntry.Id));
                });
            }
        }

        private void UpdateButtons(bool isBuilding)
        {
            editBtn.gameObject.SetActive(!isBuilding);
            saveBtn.gameObject.SetActive(isBuilding);
            cancelBtn.gameObject.SetActive(isBuilding);
        }

        private void OnClickLeaderCard()
        {
            if (string.IsNullOrEmpty(_leaderId))
            {
                return;
            }

            UIFrame.Show<CardDetailUI>(new CardDetailData(_leaderId));
        }
    }
}