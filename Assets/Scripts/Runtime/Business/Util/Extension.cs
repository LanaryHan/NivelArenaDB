using System;
using System.Linq;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Business.Util
{
    public static class Extension
    {
        public static void CloseSelfByExt(this UIPanel panel)
        {
            ExtUIManager.Instance.CloseDialog(panel);
        }

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

        public static string ToChinese(this Affiliation affiliation)
        {
            return affiliation switch
            {
                Affiliation.None => "-",
                Affiliation.Missilis => "米西利斯",
                Affiliation.Tetra => "泰特拉",
                Affiliation.Elysion => "极乐净土",
                Affiliation.Pilgrim => "朝圣者",
                Affiliation.Unique => "特殊",
                Affiliation.Successor => "继承者",
                Affiliation.Natalon => "纳特伦学院",
                Affiliation.DunBlya => "桐伯莱亚",
                Affiliation.SpiritKing => "精灵王",
                Affiliation.PoF => "过去或未来",
                Affiliation.Covenant => "圣约",
                _ => throw new ArgumentOutOfRangeException(nameof(affiliation), affiliation, null)
            };
        }

        public static string ToChinese(this KeyType keyType, string param = null)
        {
            var key = keyType switch
            {
                KeyType.None => string.Empty,
                KeyType.Entry => "入场",
                KeyType.Attacker => "攻击者",
                KeyType.Defender => "防御者",
                KeyType.Exit => "离场",
                KeyType.Passive => "被动",
                KeyType.Active => "主动",
                KeyType.Guardian => "守护者",
                KeyType.Armed => "武装",
                KeyType.LevelLink => "等级链接",
                KeyType.WireBuilding => "战线构筑",
                KeyType.Mix => "混合",
                KeyType.Credits => "借贷",
                KeyType.Escape => "逃脱",
                KeyType.ArmedCondition => "武装条件",
                KeyType.Oath => "誓约",
                KeyType.Awakening => "觉醒",
                _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
            };
            if (string.IsNullOrEmpty(param))
            {
                return key;
            }

            return $"{key}:{param}";
        }

        public static string ToDescription(this string pattern, string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return pattern;
            }

            var @params = param.Split(",");
            var args = @params.Select(p => (object) p).ToArray();
            var result = string.Format(pattern, args);
            return result;
        }
    }
}