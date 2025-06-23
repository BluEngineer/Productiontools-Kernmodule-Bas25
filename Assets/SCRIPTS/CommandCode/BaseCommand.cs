public abstract class BaseCommand
{
    public abstract void Execute();
    public abstract void Undo();
    public string CommandName { get; protected set; } = "Unnamed Command";

    // Add this property to track affected quest
    public BaseQuest AffectedQuest
    {
        get; protected set;
    }
}