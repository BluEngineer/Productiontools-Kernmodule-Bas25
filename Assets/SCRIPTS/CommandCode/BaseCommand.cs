public abstract class BaseCommand
{
    public abstract void Execute();
    public abstract void Undo();
    public string CommandName { get; protected set; } = "Unnamed Command";
}