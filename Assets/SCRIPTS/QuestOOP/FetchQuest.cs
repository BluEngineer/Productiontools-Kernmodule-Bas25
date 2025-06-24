using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Added for LINQ
using System;

[System.Serializable]
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
        clone.StartDialogueLines = new List<string>(StartDialogueLines);
        clone.CompletionDialogueLines = new List<string>(CompletionDialogueLines);
        return clone;
    }

    // FetchQuest-specific serialization hooks =====================
    public override void BeforeSerialize()
    {
        base.BeforeSerialize(); // Call base first

        // Validate and clean item requirements
        RequiredItems = RequiredItems?
            .Where(item => !string.IsNullOrWhiteSpace(item.ItemID))
            .Select(item => {
                item.ItemID = item.ItemID.Trim();
                item.Amount = Math.Max(1, item.Amount); // Ensure at least 1
                return item;
            })
            .ToList() ?? new List<ItemRequirement>();
    }

    public override void AfterDeserialize()
    {
        base.AfterDeserialize(); // Call base first

        // Initialize required items if null
        if (RequiredItems == null) RequiredItems = new List<ItemRequirement>();
    }
    // ============================================================
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