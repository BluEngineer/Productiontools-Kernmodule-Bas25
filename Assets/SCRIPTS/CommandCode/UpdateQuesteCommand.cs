public class UpdateQuestCommand : BaseCommand
{
    private readonly BaseQuest _original;
    private readonly BaseQuest _modified;

    public UpdateQuestCommand(BaseQuest original, BaseQuest modified)
    {
        _original = original;
        _modified = modified;
        CommandName = $"Update Quest: {original.Title}";
        QuestType = modified.GetType();
    }



    public override void Execute()
    {
        // Access through singleton instance
        QuestManager.Instance.ExecuteUpdateQuest(_original, _modified);
    }

    public override void Undo()
    {
        if (_original.GetType() != _modified.GetType())
        {
            int index = QuestManager.Instance.quests.IndexOf(_modified);
            if (index < 0) return;

            QuestManager.Instance.quests[index] = _original;
            AppEvents.NotifyQuestUpdated(_original);
            AppEvents.NotifyUIUpdate();
        }
        else
        {
            QuestManager.Instance.ExecuteUpdateQuest(_modified, _original);
        }
    }
}