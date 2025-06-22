public interface IQuest
{
    string QuestID
    {
        get; set;
    }
    string Title
    {
        get; set;
    }
    void AddStep(QuestStep step);
}