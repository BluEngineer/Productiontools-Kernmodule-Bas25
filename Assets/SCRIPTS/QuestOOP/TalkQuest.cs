using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Added for LINQ

[System.Serializable]
public class TalkQuest : BaseQuest, IQuest
{
    public List<string> NPCTargets { get; set; } = new List<string>();

    public TalkQuest() : base()
    {
        NPCTargets = new List<string>();
    }

    public void AddStep(QuestStep step)
    {
        Steps.Add(step);
    }

    public override BaseQuest Clone()
    {
        var clone = (TalkQuest)base.Clone();
        clone.NPCTargets = new List<string>(NPCTargets);
        clone.StartDialogueLines = new List<string>(StartDialogueLines);
        clone.CompletionDialogueLines = new List<string>(CompletionDialogueLines);
        return clone;
    }

    // TalkQuest-specific serialization hooks ======================
    public override void BeforeSerialize()
    {
        base.BeforeSerialize(); // Call base first

        // Clean NPC targets
        NPCTargets = NPCTargets?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct()
            .ToList() ?? new List<string>();
    }

    public override void AfterDeserialize()
    {
        base.AfterDeserialize(); // Call base first

        // Initialize NPC targets if null
        if (NPCTargets == null) NPCTargets = new List<string>();
    }
    // ============================================================
}