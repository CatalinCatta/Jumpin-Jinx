using System.Collections.Generic;

public enum Skin
{
    Classic,
    Goblin,
    Cat,
    Punk,
    Monkey
}


public static partial class Dictionaries
{
    public static readonly Dictionary<Skin, (int price, string name)> Skin = new()
    {
        { global::Skin.Classic, (0, "Classic") },
        { global::Skin.Goblin, (20, "Goblin") },
        { global::Skin.Cat, (50, "DarkWitch") },
        { global::Skin.Punk, (50, "Spike") },
        { global::Skin.Monkey, (50, "D.D.D.") }
    };
}