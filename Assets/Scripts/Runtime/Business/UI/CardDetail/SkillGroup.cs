using System;
using System.Linq;
using Coffee.UISoftMask;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Runtime.Business.UI.CardDetail
{
    public class SkillGroup : MonoBehaviour
    {
        [Serializable]
        public class IconGroup
        {
            public KeyType[] effectType;
            public GameObject root;
            public Image iconImage;
            public TMP_Text iconText;
            public SoftMask softMask;
            public GameObject specialFront;
            public TMP_Text descriptionText;

            public void SetActive(bool active)
            {
                root.SetActive(active);
            }

            public void SetIcon(KeyType keyType, string iconParam)
            {
                if (keyType is KeyType.None)
                {
                    return;
                }

                iconText.text = keyType.ToChinese(iconParam);
                if (keyType is KeyType.ArmedCondition)
                {
                    return;
                }

                iconImage.color = keyType switch
                {
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
                    KeyType.Mix => Color.white,
                    KeyType.Credits => "#C78800".ToRGB(),
                    KeyType.Escape => "#12A1DE".ToRGB(),
                    KeyType.Oath => Color.white,
                    KeyType.Awakening => Color.black,
                    _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
                };
                iconText.color = keyType is KeyType.Oath ? Color.black : Color.white;
                if (softMask && keyType == KeyType.Mix)
                {
                    softMask.enabled = true;
                    specialFront.SetActive(true);
                }
                else
                {
                    if (specialFront)
                    {
                        specialFront.SetActive(false);
                    }

                    if (softMask)
                    {
                        softMask.enabled = false;
                    }
                }
            }

            public void SetDescription(string pattern, string param)
            {
                descriptionText.text = pattern.ToDescription(param);
            }
        }
        
        public IconGroup[] iconGroups;

        private SkillEntry _skillEntry;

        public void Init(SkillEntry skillEntry, string descParam, string iconParam)
        {
            _skillEntry = skillEntry;
            iconGroups.ForEach(ig => ig.SetActive(ig.effectType.Contains(skillEntry.Key)));
            var activeGroup = iconGroups.First(ig => ig.effectType.Contains(skillEntry.Key));
            activeGroup.SetIcon(_skillEntry.Key, iconParam);
            if (skillEntry.Key is not KeyType.ArmedCondition)
            {
                activeGroup.SetDescription(skillEntry.Description, descParam);
            }
        }
    }
} 