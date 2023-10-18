using Newtonsoft.Json;

public class Level
{
    public string Name { get; set; }
    
    [JsonProperty("map")]
    public string[] Maps { get; set; }
    
    public int[] TimerLimitForStars;
}