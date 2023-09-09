using System.Collections.Generic;

public enum Scene
{
    Menu = -2,
    Custom,
    Endless,
    Champaign 
}

public static partial class Dictionaries
{
    public static readonly Dictionary<Scene, string> Scene = new()
    {
        { global::Scene.Menu, "StartMenu" },
        { global::Scene.Custom, "Local Game" },
        { global::Scene.Endless, "EndlessRun" },
        { global::Scene.Champaign, "Grass Lvl" }
    };
}