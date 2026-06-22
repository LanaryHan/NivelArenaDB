using System.Collections.Generic;
using System.Linq;
using Logic;
using Newtonsoft.Json;
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
        public Transform personalContent;
        public Transform presetContent;
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
            personalContent.RemoveAllChildren(addBtn.transform.parent);
            presetContent.RemoveAllChildren();
            UpdateView();
        }

        private void UpdateView()
        {
            #region Read Json(personal)

            var logic = GameRuntimeLogic.Instance.GetLogic<BuildDeckLogic>();
            var deckEntries = logic.DeckEntries?.Values.ToList() ?? new List<BuildDeckEntry>();
            foreach (var deckEntry in deckEntries)
            {
                var personalDeck = Instantiate(tmpDeck, personalContent);
                personalDeck.Init(deckEntry.DeckName,
                    deckEntry.CardIds.Select(id => DataManager.Instance.GetCard(id))
                        .First(card => card.CardType is CardType.Leader).Id, false);
                personalDeck.gameObject.SetActive(true);
            }

            addBtn.transform.parent.SetAsLastSibling();

            #endregion

            #region Read Resource(preset)

            var presetJson = Resources.Load<TextAsset>("decks").text;
            var presets = JsonConvert.DeserializeObject<Dictionary<string, BuildDeckEntry>>(presetJson);
            foreach (var deckEntry in presets.Values)
            {
                var presetDeck = Instantiate(tmpDeck, presetContent);
                presetDeck.Init(deckEntry.DeckName,
                    deckEntry.CardIds.Select(id => DataManager.Instance.GetCard(id))
                        .First(card => card.CardType is CardType.Leader).Id, true);
                presetDeck.gameObject.SetActive(true);
            }

            #endregion
        }

        protected override void OnClose()
        {
            
        }
    }
}