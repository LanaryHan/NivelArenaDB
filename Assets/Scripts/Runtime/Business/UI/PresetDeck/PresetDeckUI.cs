using System.Collections.Generic;
using System.Linq;
using Logic;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PresetDeckUI : UIPanel
    {
        public Transform content;
        public PresetDeck tmpDeck;
        public Button closeBtn;
        public Button addBtn;
        public override bool CanCloseByBackKey => true;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tmpDeck.gameObject.SetActive(false);
            closeBtn.onClick.AddListener(this.CloseSelfByExt);
            addBtn.onClick.AddListener(() =>
            {
                GetEventComponent().Send(GameEvents.SetBuildingState.Create(true, null));
                this.CloseSelfByExt();
            });
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            content.RemoveAllChildren(tmpDeck.transform, addBtn.transform.parent);
            UpdateView();
        }

        private void UpdateView()
        {
            var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
            var deckEntries = logic.DeckEntries?.Values.ToList() ?? new List<BuildDeckEntry>();
            foreach (var deckEntry in deckEntries)
            {
                var presetDeck = Instantiate(tmpDeck, content);
                presetDeck.Init(deckEntry.DeckName,
                    deckEntry.CardIds.Select(id => DataManager.Instance.GetCard(id))
                        .First(card => card.CardType is CardType.Leader).Id);
                presetDeck.gameObject.SetActive(true);
            }

            addBtn.transform.parent.SetAsLastSibling();
        }

        protected override void OnClose()
        {
            
        }
    }
}