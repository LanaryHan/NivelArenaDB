using System;
using Runtime.Business.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Business.Util
{
    public static class Extension
    {
        public static void RemoveAllChildren(this Transform parent, params Transform[] excepts)
        {
            if (parent.childCount > 0)
            {
                var index = parent.childCount;
                while (index-- > 0)
                {
                    var c = parent.GetChild(index);
                    if (excepts.Length > 0)
                    {
                        foreach (var each in excepts)
                        {
                            if (each == c)
                            {
                                goto LABEL_NEXT;
                            }
                        }
                    }

                    c.SetParent(null);
                    Object.Destroy(c.gameObject);
                    LABEL_NEXT:
                    continue;
                }
            }
        }

        public static string ToChinese(this CardType cardType)
        {
            return cardType switch
            {
                CardType.Leader => "领袖",
                CardType.Unit => "单位",
                CardType.Skill => "技能",
                CardType.Item => "装备",
                _ => throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null)
            };
        }

        public static string ToChinese(this ElementAttribute attribute)
        {
            return attribute switch
            {
                ElementAttribute.Flame => "火焰",
                ElementAttribute.Earth => "大地",
                ElementAttribute.Storm => "风暴",
                ElementAttribute.Wave => "波涛",
                ElementAttribute.Lightning => "闪电",
                _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
            };
        }
    }
}