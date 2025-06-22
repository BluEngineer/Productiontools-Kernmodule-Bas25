
using System.Collections.Generic;
using UnityEngine;

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
        return clone;
    }
}