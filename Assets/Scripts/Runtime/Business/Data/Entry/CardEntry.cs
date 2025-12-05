using JetBrains.Annotations;

namespace Runtime.Business.Data.Entry
{
    public class CardEntry
    {
        #region Csv

        /// <summary>
        /// 编号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 所属卡包
        /// </summary>
        public Deck Pack { get; set; }

        /// <summary>
        /// 稀有度
        /// </summary>
        public Rarity Rarity { get; set; }
        
        /// <summary>
        /// 卡牌类型
        /// </summary>
        public CardType CardType { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public ElementAttribute Attribute { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public int? Cost { get; set; }

        /// <summary>
        /// 战斗力
        /// </summary>
        public int? Power { get; set; }

        /// <summary>
        /// 命中
        /// </summary>
        public int? Hit { get; set; }

        /// <summary>
        /// 技能id
        /// </summary>
        [CanBeNull]
        public int[] Skills { get; set; }

        /// <summary>
        /// 所属部队
        /// </summary>
        public Affiliation Affiliation { get; set; }
        
        /// <summary>
        /// 是否有签名卡
        /// </summary>
        public bool HasSpecial { get; set; }
        
        #endregion

        public string Id => $"{Pack}-{Number}";
    }
}