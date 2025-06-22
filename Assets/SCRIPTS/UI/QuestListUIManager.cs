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

    void Start()
    {
        _addButtonText.text = "Add New Quest";
        _addQuestButton.onClick.AddListener(CreateNewQuest);
        //  AppEvents.OnQuestCreated += AddQuestItem;
        AppEvents.OnRefreshUI += RefreshUI; // Keep this
        RefreshUI();
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
        questItem.Initialize(quest);
    }
}