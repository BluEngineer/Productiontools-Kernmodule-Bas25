using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandManager : Singleton<CommandManager>
{
    [SerializeField]
    public readonly Stack<BaseCommand> _undoStack = new Stack<BaseCommand>();
    [SerializeField]
    public readonly Stack<BaseCommand> _redoStack = new Stack<BaseCommand>();


    void Update()
    {
        //if (!_isEditing) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.leftShiftKey.isPressed && keyboard.zKey.wasPressedThisFrame)
        {
            CommandManager.Instance.Undo();
        }

        if (keyboard.leftShiftKey.isPressed && keyboard.xKey.wasPressedThisFrame)
        {
            CommandManager.Instance.Redo();
        }
    }

    public void ExecuteCommand(BaseCommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);

            AppEvents.InvokeUndoRedoPerformed();

            // Refresh UI for affected quest
            if (command.AffectedQuest != null)
            {
                AppEvents.InvokeQuestAffected(command.AffectedQuest);
            }
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);

            // Notify about redo operation
            AppEvents.InvokeUndoRedoPerformed();

            // Check if command affects a quest
            if (command is BaseCommand questCommand && questCommand.AffectedQuest != null)
            {
                AppEvents.InvokeQuestAffected(questCommand.AffectedQuest);
            }
        }
    }
}