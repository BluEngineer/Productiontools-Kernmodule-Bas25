using UnityEngine;

public class QuestSelectionService : MonoBehaviour
{
    public static BaseQuest SelectedQuest
    {
        get; private set;
    }

    public static void SelectQuest(BaseQuest quest)
    {
        SelectedQuest = quest;
        AppEvents.NotifyQuestSelected(quest);
    }

    public static void ClearSelection()
    {
        SelectedQuest = null;
    }
}
