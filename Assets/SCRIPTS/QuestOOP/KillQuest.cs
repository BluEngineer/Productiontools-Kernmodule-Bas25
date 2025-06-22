// KillQuest.cs
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        return clone;
    }


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

