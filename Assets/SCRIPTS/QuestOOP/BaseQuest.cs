// BaseQuest.cs
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseQuest
{
    public string QuestID
    {
        get; set;
    }
    public string Title
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    }
    public List<QuestStep> Steps
    {
        get; set;
    }

    public string StartNPC
    {
        get; set;
    }
    public string DeliveryNPC
    {
        get; set;
    }
    public QuestReward Reward { get; set; } = new QuestReward();


    public BaseQuest()
    {
        QuestID = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
        StartNPC = string.Empty;
        DeliveryNPC = string.Empty;
        Reward = new QuestReward();

        IsActive = true;
        Steps = new List<QuestStep>();


    }

    public virtual BaseQuest Clone()
    {
        return (BaseQuest)MemberwiseClone();
    }

}

