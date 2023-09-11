using System.Collections.Generic;

public enum BuffType
{
    JumpBuff,
    SpeedBuff,
    SecondChance
}

public static partial class Dictionaries
{
    public static readonly Dictionary<BuffType, (int price, bool activatable, float multiplicator, float duration)>
        BuffDetails = new()
        {
            { BuffType.JumpBuff, (200, true, 2f, 3f) },
            { BuffType.SpeedBuff, (200, true, 1.5f, 5f) },
            { BuffType.SecondChance, (2_000, false, 0f, 0f) }
        };
}