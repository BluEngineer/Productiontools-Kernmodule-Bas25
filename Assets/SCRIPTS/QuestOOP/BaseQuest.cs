using System.Collections.Generic;
using UnityEngine;
using System; // Added for serialization

[System.Serializable]
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
    public List<string> StartDialogueLines { get; set; } = new List<string>();
    public List<string> CompletionDialogueLines { get; set; } = new List<string>();

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
        StartDialogueLines.Add("");
        CompletionDialogueLines.Add("");
    }

    public virtual BaseQuest Clone()
    {
        return (BaseQuest)MemberwiseClone();
    }

    // Serialization hooks =========================================
    public virtual void BeforeSerialize()
    {
        // Clean and validate data before saving
        Title = Title?.Trim() ?? "Untitled Quest";
        Description = Description?.Trim() ?? "";
        StartNPC = StartNPC?.Trim() ?? "";
        DeliveryNPC = DeliveryNPC?.Trim() ?? "";
        QuestID = string.IsNullOrEmpty(QuestID) ? Guid.NewGuid().ToString() : QuestID.Trim();

        // Ensure at least one step exists
        if (Steps == null || Steps.Count == 0)
        {
            Steps = new List<QuestStep> { new QuestStep() };
        }

        // Ensure at least one dialogue line exists
        if (StartDialogueLines.Count == 0) StartDialogueLines.Add("");
        if (CompletionDialogueLines.Count == 0) CompletionDialogueLines.Add("");
    }

    public virtual void AfterDeserialize()
    {
        // Initialize transient state after loading
        IsActive = true;

        // Initialize null properties
        if (Reward == null) Reward = new QuestReward();
        if (Steps == null) Steps = new List<QuestStep>();
        if (StartDialogueLines == null) StartDialogueLines = new List<string>();
        if (CompletionDialogueLines == null) CompletionDialogueLines = new List<string>();
    }
    // =============================================================
}