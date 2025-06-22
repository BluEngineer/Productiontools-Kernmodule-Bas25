
using System.Collections.Generic;
using UnityEngine;

public class FetchQuest : BaseQuest, IQuest
{
    public List<ItemRequirement> RequiredItems { get; set; } = new List<ItemRequirement>();

    public FetchQuest() : base()
    {
        RequiredItems = new List<ItemRequirement>();
    }

    public void AddStep(QuestStep step)
    {
        Steps.Add(step);
    }

    public override BaseQuest Clone()
    {
        var clone = (FetchQuest)base.Clone();
        clone.RequiredItems = new List<ItemRequirement>(RequiredItems);
        return clone;
    }
}



[System.Serializable]
public class ItemRequirement
{
    public string ItemID;
    public int Amount;

    public ItemRequirement()
    {
        ItemID = string.Empty;
        Amount = 1;
    }
}