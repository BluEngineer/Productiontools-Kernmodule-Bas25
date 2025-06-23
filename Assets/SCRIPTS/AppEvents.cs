using System;

public static class AppEvents
{
    // Quest Events
    public static event System.Action<BaseQuest> OnQuestCreated;
    public static event System.Action<BaseQuest> OnQuestDeleted;
    public static event System.Action<BaseQuest> OnQuestSelected;
    public static event System.Action<BaseQuest> OnQuestUpdated;

    // UI Events
    public static event System.Action OnRefreshUI;
    public static event Action OnUndoRedoPerformed;
    public static event Action<BaseQuest> OnQuestAffectedByUndoRedo;
    public static event Action OnCloseQuestEditor;
    public static event Action<BaseQuest> OnRefreshQuestEditor;

    public static void NotifyQuestCreated(BaseQuest quest) => OnQuestCreated?.Invoke(quest);
    public static void NotifyQuestDeleted(BaseQuest quest) => OnQuestDeleted?.Invoke(quest);
    public static void NotifyQuestSelected(BaseQuest quest) => OnQuestSelected?.Invoke(quest);
    public static void NotifyQuestUpdated(BaseQuest quest) => OnQuestUpdated?.Invoke(quest);



    public static void InvokeCloseQuestEditor() => OnCloseQuestEditor?.Invoke();
    public static void InvokeRefreshQuestEditor(BaseQuest quest) => OnRefreshQuestEditor?.Invoke(quest);
    public static void NotifyUIUpdate() => OnRefreshUI?.Invoke();
    public static void InvokeUndoRedoPerformed() => OnUndoRedoPerformed?.Invoke();
    public static void InvokeQuestAffected(BaseQuest quest) => OnQuestAffectedByUndoRedo?.Invoke(quest);
}