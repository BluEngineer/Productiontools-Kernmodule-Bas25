using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class QuestListUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _scrollContent;
    [SerializeField] private GameObject _questItemPrefab; // Should contain TMP components
    [SerializeField] private Button _addQuestButton; // Standard Unity UI Button
    [SerializeField] private TMP_Text _addButtonText; // TMP text for button

    [Header("Import/Export UI")]
    [SerializeField] private Button _exportAllButton;
    [SerializeField] private Button _importButton;
    //[SerializeField] private TMP_InputField _importPathInput;

    void Start()
    {
        _addButtonText.text = "Add New Quest";
        _addQuestButton.onClick.AddListener(CreateNewQuest);
        AppEvents.OnRefreshUI += RefreshUI;
        RefreshUI();

        _exportAllButton.onClick.AddListener(ExportAllQuests);
        _importButton.onClick.AddListener(ImportQuests);

        //QuestSerializer.Initialize();
        //why is this missing?
    }

    public void CreateNewQuest()
    {
        var newQuest = new TalkQuest { Title = "New Quest" };
        QuestManager.Instance.AddQuest(newQuest);
    }

    public void RefreshUI()
    {
        // Clear existing items
        foreach (Transform child in _scrollContent)
        {
            Destroy(child.gameObject);
        }

        // Recreate all items
        foreach (var quest in QuestManager.Instance.quests)
        {
            AddQuestItem(quest);
        }
    }

    public void AddQuestItem(BaseQuest quest)
    {
        GameObject itemObj = Instantiate(_questItemPrefab, _scrollContent);
        var questItem = itemObj.GetComponent<UIQuestListItem>();
        questItem.Initialize(quest, () => {
            // Select quest and open editor
            AppEvents.NotifyQuestSelected(quest);
        });
    }

    public void ExportAllQuests()
    {
        QuestSerializer.ExportAllQuests(QuestManager.Instance.quests);
    }

    public void ImportQuests()
    {
        // Use file browser to select import files
        List<BaseQuest> importedQuests = QuestSerializer.ImportQuests();

        // Add all imported quests to manager
        foreach (var quest in importedQuests)
        {
            QuestManager.Instance.AddQuest(quest);
        }

        Debug.Log($"Imported {importedQuests.Count} quests");
        RefreshUI();
    }
}