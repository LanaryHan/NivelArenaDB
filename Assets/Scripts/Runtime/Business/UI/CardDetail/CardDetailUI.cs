using System.Collections.Generic;
using System.Linq;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Business.UI.CardDetail
{
    public class CardDetailData : UIPanelData
    {
        public string CardId;

        public CardDetailData(string cardId)
        {
            CardId = cardId;
        }
    }
    public class CardDetailUI : UIPanel
    {
        [Header("Top")]
        public TMP_Text cardNameTxt;
        public TMP_Text cardIdTxt;
        public TMP_Text cardTypeTxt;
        public TMP_Text cardAttributeTxt;
        public RectTransform cardTarget;
        public Button closeBtn;
        [Header("Skill")] 
        public Transform skillBg;
        public SkillGroup tempSkillGroup;
        public Transform skillContent;
        [Header("CardInfo")] 
        public TMP_Text costTxt;
        public TMP_Text rareTxt;
        public TMP_Text powerTxt;
        public TMP_Text hitTxt;
        public TMP_Text affiliationTxt;
        [Header("Keywords")] 
        public TMP_Text keywordsTxt;
        [Header("Trigger")] 
        public GameObject triggerRoot;
        public TMP_Text triggerTxt;
        
        
        private CardEntry _cardEntry;
        public override bool CanCloseByBackKey => true;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempSkillGroup.gameObject.SetActive(false);
            closeBtn.onClick.AddListener(this.CloseSelfByExt);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            skillContent.RemoveAllChildren(tempSkillGroup.transform, skillBg);
            if (uiData is not CardDetailData cardData)
            {
                return;
            }

            var ec = GetEventComponent();
            var cardId = cardData.CardId;
            _cardEntry = DataManager.Instance.GetCard(cardId);
            cardNameTxt.text = _cardEntry.Name;
            cardIdTxt.text = _cardEntry.Id;
            cardTypeTxt.text = _cardEntry.CardType.ToChinese();
            cardAttributeTxt.text = _cardEntry.Attribute.ToChinese();
            costTxt.text = _cardEntry.Cost == null ? "——" : _cardEntry.Cost.Value.ToString();
            rareTxt.text = _cardEntry.Rarity.ToString();
            powerTxt.text = _cardEntry.Power == null ? "——" : _cardEntry.Power.Value.ToString();
            hitTxt.text = _cardEntry.Hit == null ? "——" : _cardEntry.Hit.Value.ToString();
            affiliationTxt.text = _cardEntry.Affiliation.ToChinese();
            UpdateSkills();
            UpdateTrigger();
            ec.Send(GameEvents.CardFollowReady.Create(cardTarget));
            ec.Send(GameEvents.ShowCard.Create(cardId));
        }

        private void UpdateSkills()
        {
            var skillIds = _cardEntry.Skills;
            var cardType = _cardEntry.CardType;
            var haveKeyword = true;
            if (skillIds == null || skillIds.Length == 0 || cardType is CardType.Skill)
            {
                keywordsTxt.text = "——";
                if (cardType is not CardType.Skill)
                {
                    skillContent.gameObject.SetActive(false);
                }
                
                haveKeyword = false;
            }

            var keywords = new HashSet<KeyType>();
            for (var i = 0; i < skillIds?.Length; i++)
            {
                var skillId = skillIds[i];
                var skillGroup = Instantiate(tempSkillGroup, skillContent);
                var skillEntry = DataManager.Instance.GetSkill(skillId);
                skillGroup.Init(skillEntry, _cardEntry.SkillParams[i], _cardEntry.IconTextParams[i]);
                skillGroup.gameObject.SetActive(true);

                if (!haveKeyword)
                {
                    continue;
                }

                if (skillEntry.Key1 is not KeyType.None)
                {
                    keywords.Add(skillEntry.Key1);
                }

                if (skillEntry.Key2 != null && skillEntry.Key2.Value is not KeyType.None)
                {
                    keywords.Add(skillEntry.Key2.Value);
                }
            }

            if (haveKeyword)
            {
                keywordsTxt.text = string.Join(", ", keywords.Select(k => k.ToChinese()));
            }
        }

        private void UpdateTrigger()
        {
            if (_cardEntry.Trigger == null)
            {
                triggerRoot.gameObject.SetActive(false);
                return;
            }
            
            var triggerId = _cardEntry.Trigger.Value;
            var triggerEntry = DataManager.Instance.GetTrigger(triggerId);
            triggerTxt.text = $"\t\t{triggerEntry.Description.ToDescription(_cardEntry.TriggerParam)}";
        }
        
        protected override void OnClose()
        {
            EventManager.Instance.Send(GameEvents.HideCard.Create());
        }
    }
}