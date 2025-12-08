using System;
using Coffee.UISoftMask;
using Runtime.Business.Data;
using Runtime.Business.Data.Entry;
using Runtime.Business.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Util;

namespace Runtime.Business.UI.CardDetail
{
    public class SkillGroup : MonoBehaviour
    {
        public Image skillNormalIcon;
        public TMP_Text skillNormalTxt;
        public Image skillLeaderIcon;
        public TMP_Text skillLeaderTxt;
        public GameObject specialFront;
        public SoftMask softMask;
        public TMP_Text skillDescription;

        public void Init(SkillEntry skillEntry)
        {
            skillDescription.text = skillEntry.Description;
            UpdateIcon(skillEntry.Key);
        }

        private void UpdateIcon(KeyType key)
        {
            var showNormal = key is not KeyType.Oath and not KeyType.Awakening;
            skillNormalIcon.gameObject.SetActive(showNormal);
            skillNormalTxt.gameObject.SetActive(showNormal);
            skillLeaderIcon.gameObject.SetActive(!showNormal);
            skillLeaderTxt.gameObject.SetActive(!showNormal);
            
            if (showNormal)
            {
                skillNormalIcon.color = key switch
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
                    KeyType.Mix => Color.white,
                    KeyType.Credits => "#C78800".ToRGB(),
                    KeyType.Escape => "#12A1DE".ToRGB(),
                    _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
                };
                skillNormalTxt.text = key.ToChinese();
                if (key is KeyType.Mix)
                {
                    softMask.enabled = true;
                    specialFront.SetActive(true);
                }
                else
                {
                    softMask.enabled = false;
                    specialFront.SetActive(false);
                }
            }
            else
            {
                (skillLeaderIcon.color, skillLeaderTxt.color) = key switch
                {
                    KeyType.Oath => (Color.white, Color.black),
                    KeyType.Awakening => (Color.black, Color.white),
                    _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
                };
                skillLeaderTxt.text = key.ToChinese();
            }
        }
    }
} 