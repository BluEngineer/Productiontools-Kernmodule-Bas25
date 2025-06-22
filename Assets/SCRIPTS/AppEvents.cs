public static class AppEvents
{
    // Quest Events
    public static event System.Action<BaseQuest> OnQuestCreated;
    public static event System.Action<BaseQuest> OnQuestDeleted;
    public static event System.Action<BaseQuest> OnQuestSelected;
    public static event System.Action<BaseQuest> OnQuestUpdated;

    // UI Events
    public static event System.Action OnRefreshUI;

    public static void NotifyQuestCreated(BaseQuest quest) => OnQuestCreated?.Invoke(quest);
    public static void NotifyQuestDeleted(BaseQuest quest) => OnQuestDeleted?.Invoke(quest);
    public static void NotifyQuestSelected(BaseQuest quest) => OnQuestSelected?.Invoke(quest);
    public static void NotifyQuestUpdated(BaseQuest quest) => OnQuestUpdated?.Invoke(quest);
    public static void NotifyUIUpdate() => OnRefreshUI?.Invoke();
}