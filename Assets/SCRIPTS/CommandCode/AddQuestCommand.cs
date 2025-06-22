public class AddQuestCommand : BaseCommand
{
    private readonly BaseQuest _quest;

    public AddQuestCommand(BaseQuest quest)
    {
        _quest = quest;
        CommandName = $"Add Quest: {quest.Title}";
    }

    public override void Execute()
    {
        // Access through singleton instance
        QuestManager.Instance.ExecuteAddQuest(_quest);
    }

    public override void Undo()
    {
        // Access through singleton instance
        QuestManager.Instance.ExecuteRemoveQuest(_quest);
    }
}