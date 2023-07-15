public static class Utils
{
    public static int RandomPickNumberExcludingZero(int max)
    {
        var random = new System.Random();

        return random.Next(1, max);
    }

    public static int RandomPickNumberBetween(int min, int max)
    {
        var random = new System.Random();

        return random.Next(min, max);
    }
}
