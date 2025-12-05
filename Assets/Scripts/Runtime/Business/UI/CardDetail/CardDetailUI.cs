using QFramework;
using Runtime.Business.Manager;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;

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
        [Header("Skill")] 
        public SkillGroup tempSkillGroup;
        public Transform skillContent;
        
        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            tempSkillGroup.gameObject.SetActive(false);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
            skillContent.RemoveAllChildren(tempSkillGroup.transform);
            if (uiData is not CardDetailData cardData)
            {
                return;
            }

            var cardId = cardData.CardId;
            var cardEntry = DataManager.Instance.GetCard(cardId);
            cardNameTxt.text = cardEntry.Name;
            cardIdTxt.text = cardEntry.Id;
            cardTypeTxt.text = cardEntry.CardType.ToChinese();
            cardAttributeTxt.text = cardEntry.Attribute.ToChinese();
            UpdateSkills(cardEntry.Skills);
        }

        private void UpdateSkills(int[] skillIds)
        {
            foreach (var skillId in skillIds)
            {
                var skillGroup = Instantiate(tempSkillGroup, skillContent);
                skillGroup.Init(skillId);
                skillGroup.gameObject.SetActive(true);
            }
        }
        
        protected override void OnClose()
        {
            
        }
    }
}