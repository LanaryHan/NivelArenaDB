using System.Text;
using QFramework;
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
        public SkillGroup tempSkillGroup;
        public Transform skillContent;
        [Header("CardInfo")] 
        public TMP_Text costTxt;
        public TMP_Text rareTxt;
        public TMP_Text powerTxt;
        public TMP_Text hitTxt;
        public TMP_Text affiliationTxt;
        public Transform keywordsContent;
        public TMP_Text tempKeyword;
        
        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempSkillGroup.gameObject.SetActive(false);
            tempKeyword.gameObject.SetActive(false);
            closeBtn.onClick.AddListener(CloseSelf);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            skillContent.RemoveAllChildren(tempSkillGroup.transform);
            keywordsContent.RemoveAllChildren(tempKeyword.transform);
            if (uiData is not CardDetailData cardData)
            {
                return;
            }

            var ec = GetEventComponent();
            var cardId = cardData.CardId;
            var cardEntry = DataManager.Instance.GetCard(cardId);
            cardNameTxt.text = cardEntry.Name;
            cardIdTxt.text = cardEntry.Id;
            cardTypeTxt.text = cardEntry.CardType.ToChinese();
            cardAttributeTxt.text = cardEntry.Attribute.ToChinese();
            costTxt.text = cardEntry.Cost == null ? "-" : cardEntry.Cost.Value.ToString();
            rareTxt.text = cardEntry.Rarity.ToString();
            powerTxt.text = cardEntry.Power == null ? "-" : cardEntry.Power.Value.ToString();
            hitTxt.text = cardEntry.Hit == null ? "-" : cardEntry.Hit.Value.ToString();
            affiliationTxt.text = cardEntry.Affiliation.ToChinese();
            UpdateSkills(cardEntry.Skills);
            ec.Send(GameEvents.CardFollowReady.Create(cardTarget));
            ec.Send(GameEvents.ShowCard.Create(cardId));
        }

        private void UpdateSkills(int[] skillIds)
        {
            if (skillIds == null || skillIds.Length == 0)
            {
                var keywordTxt = Instantiate(tempKeyword, keywordsContent);
                keywordTxt.text = "-";
                keywordTxt.gameObject.SetActive(true);
                return;
            }

            for (var i = 0; i < skillIds.Length; i++)
            {
                var skillId = skillIds[i];
                var skillGroup = Instantiate(tempSkillGroup, skillContent);
                var skillEntry = DataManager.Instance.GetSkill(skillId);
                skillGroup.Init(skillEntry);
                skillGroup.gameObject.SetActive(true);

                var keywordTxt = Instantiate(tempKeyword, keywordsContent);
                var keyText = GetKeyText(skillEntry);
                keywordTxt.text = keyText + (i == skillIds.Length - 1 ? "" : ",");
                keywordTxt.gameObject.SetActive(true);
            }
        }

        private string GetKeyText(SkillEntry entry)
        {
            var builder = new StringBuilder();
            builder.Append($"{entry.Key.ToChinese()}");
            if (!string.IsNullOrEmpty(entry.Param))
            {
                builder.Append($":{entry.Param}");
            }
            
            return builder.ToString();
        }
        
        protected override void OnClose()
        {
            EventManager.Instance.Send(GameEvents.HideCard.Create());
        }
    }
}