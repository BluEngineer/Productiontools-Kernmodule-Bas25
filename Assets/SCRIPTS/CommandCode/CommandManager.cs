using System.Collections.Generic;
using UnityEngine;

public class CommandManager : Singleton<CommandManager>
{
    [SerializeField]
    public readonly Stack<BaseCommand> _undoStack = new Stack<BaseCommand>();
    [SerializeField]
    public readonly Stack<BaseCommand> _redoStack = new Stack<BaseCommand>();

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
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
        }
    }
}