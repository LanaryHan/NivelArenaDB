using System;
using System.Linq;
using QFramework;
using Runtime.Business.Data;
using Runtime.Business.Manager;
using UI;
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

        public static void HideSelfByExt(this UIPanel panel)
        {
            ExtUIManager.Instance.HideDialog(panel);
        }

        public static void ShowSelfByExt(this UIPanel panel)
        {
            ExtUIManager.Instance.ShowDialog(panel);
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
                Affiliation.Homunculus => "人造人",
                Affiliation.RunwayFighter => "走秀格斗家",
                Affiliation.Visitors => "来自其他宇宙的访客",
                Affiliation.Skuggiheim => "斯库基黑",
                Affiliation.Opponents => "敌对者",
                Affiliation.SummerSP => "夏日限定",
                Affiliation.Royal => "皇室",
                Affiliation.Reingar => "雷茵格尔",
                Affiliation.Politia => "佛里蒂亚",
                Affiliation.Unknown => "未知世界",
                Affiliation.Refundos => "雷逢朵斯",
                Affiliation.ColdDays => "冬日",
                Affiliation.CotO => "大洋征服者",
                Affiliation.WhoMadeIt => "被创造的存在",
                Affiliation.Geralt => "圣杰特拉",
                Affiliation.DragonValley => "龙之溪谷",
                Affiliation.Wintenberg => "万坦贝克",
                Affiliation.SecretGarden => "秘密花园",
                Affiliation.FlawlessCity => "完美都市",
                Affiliation.SFP => "幻影队",
                Affiliation.FallLand => "腐朽大地",
                Affiliation.Perland => "普兰特",
                Affiliation.Anniversary => "周年纪念",
                Affiliation.School => "军官学园",
                Affiliation.Test => "实验体",
                Affiliation.Festival => "祭典",
                Affiliation.Change => "转变",
                Affiliation.Valentine => "情人节",
                Affiliation.Legion => "炼狱",
                Affiliation.Splash => "逐浪",
                Affiliation.Sunset => "晚霞",
                Affiliation.GhostHunter => "幽灵猎人",
                Affiliation.Prison => "监狱",
                Affiliation.Pajamas => "睡衣派对",
                Affiliation.Lucky => "好运",
                Affiliation.Snow => "冰雪",
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

        private static int LowestBit(int value)
        {
            for (var i = 0; i < 32; i++)
            {
                if ((value & (1 << i)) != 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public static string ToChinese(this Enum @enum)
        {
            switch (@enum)
            {
                case AttributeFlags attributeFlags:
                {
                    var i = LowestBit((int)attributeFlags);
                    var element = (ElementAttribute)i;
                    return element.ToChinese();
                }
                case KeywordFlags keywordFlags:
                {
                    var i = LowestBit((int)keywordFlags);
                    var keyType = (KeyType)(i + 1);
                    return keyType.ToChinese();
                }
                case CostFlags costFlags:
                    return costFlags switch
                    {
                        CostFlags.None => string.Empty,
                        CostFlags.Zero => "0",
                        CostFlags.One => "1",
                        CostFlags.Two => "2",
                        CostFlags.Three => "3",
                        CostFlags.Four => "4",
                        CostFlags.Five => "5",
                        CostFlags.Six => "6",
                        CostFlags.Seven => "7",
                        CostFlags.Eight => "8",
                        CostFlags.Nine => "9",
                        CostFlags.TenPlus => "10+",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                case CardTypeFlags cardTypeFlags:
                {
                    var i = LowestBit((int)cardTypeFlags);
                    var type = (CardType)i;
                    return type.ToChinese();
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(@enum), @enum, null);
            }
        }
    }
}