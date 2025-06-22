using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEditorUIManager : MonoBehaviour
{
    [Header("Core UI")]
    [SerializeField] private GameObject _editorPanel;
    [SerializeField] private TMP_InputField _titleInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private Button _saveButton;
    [SerializeField] private TMP_Dropdown _questTypeDropdown;

    [Header("Type Panels")]
    [SerializeField] private GameObject _talkPanel;
    [SerializeField] private GameObject _fetchPanel;
    [SerializeField] private GameObject _killPanel;

    [Header("Talk Quest UI")]
    [SerializeField] private Transform _talkListContainer;
    [SerializeField] private GameObject _talkItemPrefab;
    [SerializeField] private Button _addTalkItemButton;

    [Header("Fetch Quest UI")]
    [SerializeField] private Transform _fetchListContainer;
    [SerializeField] private GameObject _fetchItemPrefab;
    [SerializeField] private Button _addFetchItemButton;

    [Header("Kill Quest UI")]
    [SerializeField] private Transform _killListContainer;
    [SerializeField] private GameObject _killItemPrefab;
    [SerializeField] private Button _addKillItemButton;

    private BaseQuest _originalQuest;
    private bool _isEditing;

    void Start()
    {
        _saveButton.onClick.AddListener(SaveChanges);
        AppEvents.OnQuestSelected += OpenEditor;
        _editorPanel.SetActive(false);

        // Setup dropdown
        _questTypeDropdown.ClearOptions();
        _questTypeDropdown.AddOptions(new List<string> {
            "Talk Quest", "Fetch Quest", "Kill Quest"
        });
        _questTypeDropdown.onValueChanged.AddListener(OnQuestTypeChanged);

        // Setup buttons
        //_addTalkItemButton.onClick.AddListener(AddTalkItem);
        //_addFetchItemButton.onClick.AddListener(AddFetchItem);
        //_addKillItemButton.onClick.AddListener(AddKillItem);
    }
  

    private void OpenEditor(BaseQuest quest)
    {
        _originalQuest = quest;
        _isEditing = true;
        _editorPanel.SetActive(true);

        // Clear existing items
        ClearList(_talkListContainer);
        ClearList(_fetchListContainer);
        ClearList(_killListContainer);

        // Set common fields
        _titleInput.text = quest.Title;
        _descriptionInput.text = quest.Description;

        // Set quest type and load data
        if (quest is TalkQuest talkQuest)
        {
            _questTypeDropdown.value = 0;
            LoadTalkData(talkQuest);
        }
        else if (quest is FetchQuest fetchQuest)
        {
            _questTypeDropdown.value = 1;
            LoadFetchData(fetchQuest);
        }
        else if (quest is KillQuest killQuest)
        {
            _questTypeDropdown.value = 2;
            LoadKillData(killQuest);
        }

        UpdateActivePanel();
    }

    private void UpdateActivePanel()
    {
        _talkPanel.SetActive(_questTypeDropdown.value == 0);
        _fetchPanel.SetActive(_questTypeDropdown.value == 1);
        _killPanel.SetActive(_questTypeDropdown.value == 2);
    }

    private void OnQuestTypeChanged(int typeIndex)
    {
        if (!_isEditing) return;
        UpdateActivePanel();
    }

    private void LoadTalkData(TalkQuest quest)
    {
        foreach (string npcId in quest.NPCTargets)
        {
            AddTalkItem(npcId);
        }
    }

    private void LoadFetchData(FetchQuest quest)
    {
        foreach (var requirement in quest.RequiredItems)
        {
            AddFetchItem(requirement.ItemID, requirement.Amount);
        }
    }

    private void LoadKillData(KillQuest quest)
    {
        foreach (var target in quest.Targets)
        {
            AddKillItem(target.EnemyID, target.Count);
        }
    }
    // added empty wrapppper methods for use with buttons
    public void AddEmptyFetchItem() => AddFetchItem("", 1);
    public void AddEmptyKillItem() => AddKillItem("", 1);

    public void AddTalkItem(string npcId = "")
    {
        var item = Instantiate(_talkItemPrefab, _talkListContainer);
        var input = item.GetComponentInChildren<TMP_InputField>();
        if (input != null) input.text = npcId;
    }

    public void AddFetchItem(string itemId = "", int amount = 1)
    {
        var item = Instantiate(_fetchItemPrefab, _fetchListContainer);
        var inputs = item.GetComponentsInChildren<TMP_InputField>();
        if (inputs != null && inputs.Length >= 2)
        {
            inputs[0].text = itemId;
            inputs[1].text = amount.ToString();
        }
    }

    public void AddKillItem(string enemyId = "", int count = 1)
    {
        var item = Instantiate(_killItemPrefab, _killListContainer);
        var inputs = item.GetComponentsInChildren<TMP_InputField>();
        if (inputs != null && inputs.Length >= 2)
        {
            inputs[0].text = enemyId;
            inputs[1].text = count.ToString();
        }
    }

    private void ClearList(Transform container)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            if (!child.CompareTag("ProtectedUI")) // Tag your buttons
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void SaveChanges()
    {
        if (_originalQuest == null) return;

        BaseQuest modifiedQuest;

        // Create new quest of SELECTED TYPE if type changed
        if (ShouldChangeQuestType())
        {
            modifiedQuest = CreateNewQuestOfSelectedType();
        }
        else
        {
            modifiedQuest = _originalQuest.Clone();
        }

        // Update common properties
        modifiedQuest.Title = _titleInput.text;
        modifiedQuest.Description = _descriptionInput.text;

        // Update type-specific data (using current UI type)
        switch (_questTypeDropdown.value)
        {
            case 0: SaveTalkData((TalkQuest)modifiedQuest); break;
            case 1: SaveFetchData((FetchQuest)modifiedQuest); break;
            case 2: SaveKillData((KillQuest)modifiedQuest); break;
        }

        QuestManager.Instance.UpdateQuest(_originalQuest, modifiedQuest);
        CloseEditor();
    }
    private bool ShouldChangeQuestType()
    {
        return (_originalQuest is TalkQuest && _questTypeDropdown.value != 0) ||
               (_originalQuest is FetchQuest && _questTypeDropdown.value != 1) ||
               (_originalQuest is KillQuest && _questTypeDropdown.value != 2);
    }
    private BaseQuest CreateNewQuestOfSelectedType()
    {
        return _questTypeDropdown.value switch
        {
            0 => new TalkQuest(),
            1 => new FetchQuest(),
            2 => new KillQuest(),
            _ => throw new System.NotImplementedException()
        };
    }



    private void SaveTalkData(TalkQuest quest)
    {
        quest.NPCTargets.Clear();
        foreach (Transform child in _talkListContainer)
        {
            var input = child.GetComponentInChildren<TMP_InputField>();
            if (input != null && !string.IsNullOrEmpty(input.text))
            {
                quest.NPCTargets.Add(input.text);
            }
        }
    }

    private void SaveFetchData(FetchQuest quest)
    {
        quest.RequiredItems.Clear();
        foreach (Transform child in _fetchListContainer)
        {
            var fields = child.GetComponentsInChildren<TMP_InputField>();
            if (fields != null && fields.Length >= 2 && !string.IsNullOrEmpty(fields[0].text))
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

    private void SaveKillData(KillQuest quest)
    {
        quest.Targets.Clear();
        foreach (Transform child in _killListContainer)
        {
            var fields = child.GetComponentsInChildren<TMP_InputField>();
            if (fields != null && fields.Length >= 2 && !string.IsNullOrEmpty(fields[0].text))
            {
                int count = int.TryParse(fields[1].text, out int result) ? result : 1;
                quest.Targets.Add(new EnemyTarget
                {
                    EnemyID = fields[0].text,
                    Count = count
                });
            }
        }
    }

    private void CloseEditor()
    {
        _isEditing = false;
        _editorPanel.SetActive(false);
        _originalQuest = null;
    }

    private void OnDestroy()
    {
        AppEvents.OnQuestSelected -= OpenEditor;
    }
}