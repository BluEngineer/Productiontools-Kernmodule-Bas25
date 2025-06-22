// QuestStep.cs (new file)
using UnityEngine;

[System.Serializable]
public class QuestStep
{
    public string StepID;
    public string Instructions;
    public bool IsCompleted;

    public QuestStep()
    {
        StepID = string.Empty;
        Instructions = string.Empty;
        IsCompleted = false;
    }
}