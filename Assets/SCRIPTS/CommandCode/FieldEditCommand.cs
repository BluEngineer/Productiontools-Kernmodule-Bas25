using System;
using UnityEngine;

public class FieldEditCommand : BaseCommand
{
    private readonly Action _execute;
    private readonly Action _undo;
    private readonly string _fieldName;
    private readonly BaseQuest _affectedQuest;

    public FieldEditCommand(Action execute, Action undo, string fieldName, BaseQuest quest)
    {
        _execute = execute;
        _undo = undo;
        _fieldName = fieldName;
        _affectedQuest = quest;
        CommandName = $"Edit {fieldName}";
        AffectedQuest = quest; // Set for UI updates
    }

    public override void Execute()
    {
        _execute?.Invoke();
        AppEvents.NotifyQuestUpdated(_affectedQuest);
    }

    public override void Undo()
    {
        _undo?.Invoke();
        AppEvents.NotifyQuestUpdated(_affectedQuest);
    }
}