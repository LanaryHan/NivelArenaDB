using System.Collections.Generic;
using System.Linq;
using Logic;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UIEvents;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeckEditorData : UIPanelData
    {
        public string DeckName;

        public DeckEditorData(string deckName)
        {
            DeckName = deckName;
        }
    }
    public class DeckEditorUI : UIPanel
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
        public override bool CanCloseByBackKey => true;
        private string _leaderId;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempCard.gameObject.SetActive(false);
            var ec = GetEventComponent();
            ec.Listen<OnDialogClose>(evt =>
            {
                if (evt.Dialog is Dialog.Card_Details_UI)
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
            closeBtn.onClick.AddListener(this.CloseSelfByExt);
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
            cancelBtn.onClick.AddListener(OnClickCancel);
            leaderCard.onClick.AddListener(OnClickLeaderCard);
        }
        
        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            if (mUIData is not DeckEditorData deckEditorData)
            {
                return;
            }

            var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
            if (logic.IsBuilding)
            {
                UpdateViewBuilding();
            }
            else
            {
                UpdateView(logic.GetDeckEntry(deckEditorData.DeckName));
            }

            UpdateButtons(logic.IsBuilding);
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
                            EventManager.Instance.Send(GameEvents.SetBuildingState.Create(false, this.CloseSelfByExt));
                        }

                        u.CloseSelfByExt();
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
                var sprite = DataManager.Instance.LoadCardSprite(leaderCard.Id);
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
                var cardSprite = DataManager.Instance.LoadCardSprite(cardEntry.Id);
                btn.image.sprite = cardSprite;
                btn.gameObject.SetActive(true);
                btn.onClick.AddListener(() =>
                {
                    ExtUIManager.Instance.OpenDialog<CardDetailUI>(Dialog.Card_Details_UI, new CardDetailData(cardEntry.Id));
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

            ExtUIManager.Instance.OpenDialog<CardDetailUI>(Dialog.Card_Details_UI, new CardDetailData(_leaderId));
        }
        protected override void OnClose()
        {
        }
    }
}