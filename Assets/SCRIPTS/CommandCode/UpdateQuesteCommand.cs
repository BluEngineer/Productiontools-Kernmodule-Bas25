public class UpdateQuestCommand : BaseCommand
{
    private readonly BaseQuest _original;
    private readonly BaseQuest _modified;

    public UpdateQuestCommand(BaseQuest original, BaseQuest modified)
    {
        _original = original;
        _modified = modified;
        CommandName = $"Update Quest: {original.Title}";
    }

    public override void Execute()
    {
        // Access through singleton instance
        QuestManager.Instance.ExecuteUpdateQuest(_original, _modified);
    }

    public override void Undo()
    {
        // Access through singleton instance
        QuestManager.Instance.ExecuteUpdateQuest(_modified, _original);
    }
}