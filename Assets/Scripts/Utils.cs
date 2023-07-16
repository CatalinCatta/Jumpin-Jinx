using System;

public static class Utils
{
    private readonly static Random Random = new(); 
    
    public static int RandomPickNumberExcludingZero(int max) =>
        Random.Next(1, max);

    public static int RandomPickNumberBetween(int min, int max) =>
        Random.Next(min, max);
    
    public static string DoubleToString(double number) => number switch
    {
        >= 100_000_000_000_000 => (number / 1_000_000_000_000).ToString("F0") + " t",

        >= 10_000_000_000_000 => (number / 1_000_000_000_000).ToString("F1") + " t",
        
        >= 1_000_000_000_000 => (number / 1_000_000_000_000).ToString("F2") + " t",

        >= 100_000_000_000 => (number / 1_000_000_000).ToString("F0") + " b",
        
        >= 10_000_000_000 => (number / 1_000_000_000).ToString("F1") + " b",
        
        >= 1_000_000_000 => (number / 1_000_000_000).ToString("F2") + " b",

        >= 100_000_000 => (number / 1_000_000).ToString("F0") + " m",
        
        >= 10_000_000 => (number / 1_000_000).ToString("F1") + " m",
        
        >= 1_000_000 => (number / 1_000_000).ToString("F2") + " m",

        >= 100_000 => (number / 1_000).ToString("F0") + " k",
        
        >= 10_000 => (number / 1_000).ToString("F1") + " k",
        
        >= 1_000 => (number / 1_000).ToString("F2") + " k",

        >= 100 => number.ToString("F0") + " ",

        >= 10 => number.ToString("F1") + " ",
        
        0 => "0 ",

        _ => number.ToString("F2") + " "

    };
}
