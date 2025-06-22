using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField]
    public List<BaseQuest> quests = new List<BaseQuest>();

    public void AddQuest(BaseQuest quest)
    {
        CommandManager.Instance.ExecuteCommand(new AddQuestCommand(quest));
    }

    public void DeleteQuest(BaseQuest quest)
    {
        CommandManager.Instance.ExecuteCommand(new DeleteQuestCommand(quest));
    }

    public void UpdateQuest(BaseQuest original, BaseQuest modified)
    {
        CommandManager.Instance.ExecuteCommand(new UpdateQuestCommand(original, modified));
    }

    public void ExecuteAddQuest(BaseQuest quest)
    {
        quests.Add(quest);
        Debug.Log($"Added quest: {quest.Title}");
        // ADD THIS:
        AppEvents.NotifyQuestCreated(quest);
        AppEvents.NotifyUIUpdate();
    }

    public void ExecuteRemoveQuest(BaseQuest quest)
    {
        quests.Remove(quest);
        Debug.Log($"Removed quest: {quest.Title}");
        // Trigger UI update
        AppEvents.NotifyQuestDeleted(quest);
        AppEvents.NotifyUIUpdate();
    }

    public void ExecuteInsertQuest(int index, BaseQuest quest)
    {
        quests.Insert(index, quest);
        Debug.Log($"Inserted quest at {index}: {quest.Title}");
        // Trigger UI update
        AppEvents.NotifyQuestCreated(quest); // Or use NotifyUIUpdate
        AppEvents.NotifyUIUpdate();
    }

    public void ExecuteUpdateQuest(BaseQuest original, BaseQuest modified)
    {
        int index = quests.IndexOf(original);
        if (index >= 0)
        {
            quests[index] = modified;
            Debug.Log($"Updated quest: {original.Title}");
            // Trigger UI update
            AppEvents.NotifyQuestUpdated(modified);
            AppEvents.NotifyUIUpdate();
        }
    }
}