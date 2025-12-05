using System;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Runtime.Business.UI.CardDetail
{
    public class SkillGroup : MonoBehaviour
    {
        public Image skillTypeIcon;
        public TMP_Text skillDescription;

        public void Init(int skillId)
        {
            var skillEntry = DataManager.Instance.GetSkill(skillId);
            skillDescription.text = skillEntry.Description;
            UpdateIcon(skillEntry.Key);
        }

        private void UpdateIcon(KeyType key)
        {
            skillTypeIcon.color = key switch
            {
                KeyType.None => Color.white,
                KeyType.Entry => "#EE7325".ToRGB(),
                KeyType.Attacker => "#C30D23".ToRGB(),
                KeyType.Defender => "#028554".ToRGB(),
                KeyType.Exit => "#555455".ToRGB(),
                KeyType.Passive => "#601886".ToRGB(),
                KeyType.Active => "#0D70B8".ToRGB(),
                KeyType.Guardian => "#0FA0A1".ToRGB(),
                KeyType.Armed => "#6F3F20".ToRGB(),
                KeyType.LevelLink => "#5BAB46".ToRGB(),
                KeyType.FrontConstruction => "#0F5182".ToRGB(),
                // KeyType.Oath => expr,
                // KeyType.Awakening => expr,
                _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
            };
        }
    }
} 