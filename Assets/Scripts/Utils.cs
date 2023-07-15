using System;

public static class Utils
{
    private readonly static Random Random = new(); 
    
    public static int RandomPickNumberExcludingZero(int max) =>
        Random.Next(1, max);

    public static int RandomPickNumberBetween(int min, int max) =>
        Random.Next(min, max);
}
