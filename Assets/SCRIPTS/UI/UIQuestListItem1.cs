
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestListItem1 : MonoBehaviour
{

[Header("TMP References")]
[SerializeField] private TMP_Text _titleText;
[SerializeField] private TMP_Text _typeText;
[SerializeField] private Button _selectButton; // Standard Unity UI Button
[SerializeField] private Button _deleteButton; // Standard Unity UI Button
[SerializeField] private TMP_Text _selectButtonText;
[SerializeField] private TMP_Text _deleteButtonText;

private BaseQuest _linkedQuest;

public void Initialize(BaseQuest quest)
{
    _linkedQuest = quest;

    // Set TMP texts
    _titleText.text = string.IsNullOrEmpty(quest.Title) ? "New Quest" : quest.Title;
    _typeText.text = GetQuestType(quest);
    _selectButtonText.text = "Edit";
    _deleteButtonText.text = "Delete";

    // Add button listeners
    _selectButton.onClick.AddListener(OnSelect);
    _deleteButton.onClick.AddListener(OnDelete);
}

private string GetQuestType(BaseQuest quest)
{
    if (quest is TalkQuest) return "Talk Quest";
    if (quest is FetchQuest) return "Fetch Quest";
    if (quest is KillQuest) return "Kill Quest";
    return "Unknown Type";
}

private void OnSelect()
{
    AppEvents.NotifyQuestSelected(_linkedQuest);
}

private void OnDelete()
{
    QuestManager.Instance.DeleteQuest(_linkedQuest);
}

public void UpdateTitle(string newTitle)
{
    _titleText.text = newTitle;
}
}