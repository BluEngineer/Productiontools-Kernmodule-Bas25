using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FetchQuestPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Button _addItemButton;

    private FetchQuest _currentQuest;

    void Start()
    {
        _addItemButton.onClick.AddListener(OnAddItem);
    }

    public void LoadData(FetchQuest quest)
    {
        _currentQuest = quest;
        ClearList();

        foreach (var requirement in quest.RequiredItems)
        {
            AddItem(requirement.ItemID, requirement.Amount);
        }
    }

    public void SaveData(FetchQuest quest)
    {
        quest.RequiredItems.Clear();

        foreach (Transform item in _itemsContainer)
        {
            // Skip the add button
            if (item.gameObject == _addItemButton.gameObject) continue;

            var fields = item.GetComponentsInChildren<TMP_InputField>();
            if (fields.Length >= 2 && !string.IsNullOrEmpty(fields[0].text))
            {
                int amount = int.TryParse(fields[1].text, out int result) ? result : 1;
                quest.RequiredItems.Add(new ItemRequirement
                {
                    ItemID = fields[0].text,
                    Amount = amount
                });
            }
        }
    }

    private void AddItem(string itemId = "", int amount = 1)
    {
        var item = Instantiate(_itemPrefab, _itemsContainer);
        var fields = item.GetComponentsInChildren<TMP_InputField>();

        if (fields.Length >= 2)
        {
            fields[0].text = itemId;
            fields[1].text = amount.ToString();

            // Add change listeners
            fields[0].onEndEdit.AddListener((value) => CreateChangeCommand());
            fields[1].onEndEdit.AddListener((value) => CreateChangeCommand());
        }

        // Add delete button functionality
        var deleteButton = item.GetComponentInChildren<Button>();
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(() => {
                BaseQuest beforeChange = _currentQuest.Clone();
                Destroy(item.gameObject);
                SaveData(_currentQuest);
                var command = new UpdateQuestCommand(beforeChange, _currentQuest);
                CommandManager.Instance.ExecuteCommand(command);
            });
        }
    }

    private void CreateChangeCommand()
    {
        BaseQuest beforeChange = _currentQuest.Clone();
        SaveData(_currentQuest);
        var command = new UpdateQuestCommand(beforeChange, _currentQuest);
        CommandManager.Instance.ExecuteCommand(command);
    }

    public void ClearList()
    {
        // Only destroy list items, not the add button
        for (int i = _itemsContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = _itemsContainer.GetChild(i);
            if (child.gameObject != _addItemButton.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void OnAddItem()
    {
        BaseQuest beforeChange = _currentQuest.Clone();
        AddItem();
        SaveData(_currentQuest);
        var command = new UpdateQuestCommand(beforeChange, _currentQuest);
        CommandManager.Instance.ExecuteCommand(command);
    }
}