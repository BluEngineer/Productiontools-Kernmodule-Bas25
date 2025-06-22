public abstract class BaseCommand
{
    public abstract void Execute();
    public abstract void Undo();
    public string CommandName { get; protected set; } = "Unnamed Command";

    // New: Track quest type for better undo/redo
    public System.Type QuestType
    {
        get; protected set;
    }
}