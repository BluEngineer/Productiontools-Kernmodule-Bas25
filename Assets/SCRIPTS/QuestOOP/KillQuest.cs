using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Linq; // Added for LINQ
using System;


[System.Serializable]
public class KillQuest : BaseQuest, IQuest
{
    public List<EnemyTarget> Targets { get; set; } = new List<EnemyTarget>();

    public KillQuest() : base()
    {
        Targets = new List<EnemyTarget>();
    }

    public void AddStep(QuestStep step)
    {
        Steps.Add(step);
    }

    public override BaseQuest Clone()
    {
        var clone = (KillQuest)base.Clone();
        clone.Targets = new List<EnemyTarget>(Targets);
        clone.StartDialogueLines = new List<string>(StartDialogueLines);
        clone.CompletionDialogueLines = new List<string>(CompletionDialogueLines);
        return clone;
    }

    // KillQuest-specific serialization hooks ======================
    public override void BeforeSerialize()
    {
        base.BeforeSerialize(); // Call base first

        // Validate and clean enemy targets
        Targets = Targets?
            .Where(target => !string.IsNullOrWhiteSpace(target.EnemyID))
            .Select(target => {
                target.EnemyID = target.EnemyID.Trim();
                target.Count = Math.Max(1, target.Count); // Ensure at least 1
                return target;
            })
            .ToList() ?? new List<EnemyTarget>();
    }

    public override void AfterDeserialize()
    {
        base.AfterDeserialize(); // Call base first

        // Initialize targets if null
        if (Targets == null) Targets = new List<EnemyTarget>();
    }
    // ============================================================
}

[System.Serializable]
public class EnemyTarget
{
    public string EnemyID;
    public int Count;

    public EnemyTarget()
    {
        EnemyID = string.Empty;
        Count = 1;
    }
}