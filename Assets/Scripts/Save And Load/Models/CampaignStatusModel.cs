using System;

/// <summary>
/// Represents the model containing all important statuses from campaign lvl.
/// </summary>
[Serializable]
public class CampaignStatusModel
{
    public int maxStarNrObtained;
    public float bestTime;
    public bool completed;
    
    public CampaignStatusModel(bool completed, int? stars = 0, float? time = 0)
    {
        maxStarNrObtained = (int)stars;
        bestTime = (float)time;
        this.completed = completed;
    }
}