using Newtonsoft.Json;

public class Level
{
    [JsonProperty("map")]
    public string[] Maps { get; set; }

    public int[] TimerLimitForStars { get; set; } =
    {
        10_000, 10_000, 10_000
    };
}