public class DeleteQuestCommand : BaseCommand
{
    private readonly BaseQuest _quest;
    private int _index;

    public DeleteQuestCommand(BaseQuest quest)
    {
        _quest = quest;
        CommandName = $"Delete Quest: {quest.Title}";
        AffectedQuest = quest; // Track affected quest
    }

    public override void Execute()
    {
        // Access list directly from singleton instance
        _index = QuestManager.Instance.quests.IndexOf(_quest);
        QuestManager.Instance.ExecuteRemoveQuest(_quest);
    }

    public override void Undo()
    {
        if (_index >= 0 && _index <= QuestManager.Instance.quests.Count)
        {
            QuestManager.Instance.ExecuteInsertQuest(_index, _quest);
        }
    }
}