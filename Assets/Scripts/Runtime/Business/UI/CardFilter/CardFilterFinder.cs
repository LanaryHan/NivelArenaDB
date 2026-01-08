using System;

namespace UI
{
    [Flags]
    public enum AttributeFlags
    {
        None = 0,
        Flame = 1 << 0,
        Earth = 1 << 1,
        Storm = 1 << 2,
        Wave = 1 << 3,
        Light = 1 << 4,
    }

    [Flags]
    public enum CardTypeFlags
    {
        None = 0,
        Leader = 1 << 0,
        Unit = 1 << 1,
        Skill = 1 << 2,
        Item = 1 << 3,
    }

    [Flags]
    public enum CostFlags
    {
        None = 0,
        Zero = 1 << 0,
        One = 1 << 1,
        Two = 1 << 2,
        Three = 1 << 3,
        Four = 1 << 4,
        Five = 1 << 5,
        Six = 1 << 6,
        Seven = 1 << 7,
        Eight = 1 << 8,
        Nine = 1 << 9,
        TenPlus = 1 << 10,
    }

    [Flags]
    public enum KeywordFlags
    {
        None = 0,
        Entry = 1 << 0,
        Attacker = 1 << 1,
        Defender = 1 << 2,
        Exit = 1 << 3,
        Passive = 1 << 4,
        Active = 1 << 5,
        Guardian = 1 << 6,
        Armed = 1 << 7,
        LevelLink = 1 << 8,
        WireBuilding = 1 << 9,
        Mix = 1 << 10,
        Credits = 1 << 11,
        Escape = 1 << 12,
    }
}