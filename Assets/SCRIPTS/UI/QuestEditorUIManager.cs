using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEditorUIManager : MonoBehaviour
{
    [Header("Main Editor UI")]
    [SerializeField] private GameObject _editorPanel;
    [SerializeField] private TMP_InputField _titleInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private Button _saveButton;
    [SerializeField] private TMP_Dropdown _questTypeDropdown;

    [Header("Type-Specific Panels")]
    [SerializeField] private GameObject _talkPanel;
    [SerializeField] private GameObject _fetchPanel;
    [SerializeField] private GameObject _killPanel;

    // Panel controllers
    private TalkQuestPanelController _talkController;
    private FetchQuestPanelController _fetchController;
    private KillQuestPanelController _killController;

    // Container references (assign in Inspector)
    [Header("List Containers")]
    [SerializeField] private Transform _talkListContainer;
    [SerializeField] private Transform _fetchListContainer;
    [SerializeField] private Transform _killListContainer;

    private BaseQuest _currentQuest;
    private BaseQuest _originalState;
    private bool _isEditing;

    void Start()
    {
        _saveButton.onClick.AddListener(SaveChanges);
        AppEvents.OnQuestSelected += OpenEditor;
        _editorPanel.SetActive(false);

        // Initialize panel controllers
        _talkController = _talkPanel.GetComponent<TalkQuestPanelController>();
        _fetchController = _fetchPanel.GetComponent<FetchQuestPanelController>();
        _killController = _killPanel.GetComponent<KillQuestPanelController>();

        // Setup dropdown options
        _questTypeDropdown.ClearOptions();
        _questTypeDropdown.AddOptions(new List<string> {
            "Talk Quest",
            "Fetch Quest",
            "Kill Quest"
        });
        _questTypeDropdown.onValueChanged.AddListener(OnQuestTypeChanged);

        // Add input field change listeners
        _titleInput.onEndEdit.AddListener(_ => OnInputFieldChanged());
        _descriptionInput.onEndEdit.AddListener(_ => OnInputFieldChanged());
    }

    private void OpenEditor(BaseQuest quest)
    {
        _currentQuest = quest;
        _originalState = quest.Clone();
        _titleInput.text = quest.Title;
        _descriptionInput.text = quest.Description;
        _editorPanel.SetActive(true);
        _isEditing = true;

        // Activate correct panel based on quest type
        UpdateActivePanel();

        // Set dropdown to current type
        SetDropdownValue();
    }

    private void UpdateActivePanel()
    {
        // Deactivate all panels first
        _talkPanel.SetActive(false);
        _fetchPanel.SetActive(false);
        _killPanel.SetActive(false);

        // Activate the correct panel and load data
        if (_currentQuest is TalkQuest talkQuest)
        {
            _talkPanel.SetActive(true);
            _talkController.LoadData(talkQuest);
        }
        else if (_currentQuest is FetchQuest fetchQuest)
        {
            _fetchPanel.SetActive(true);
            _fetchController.LoadData(fetchQuest);
        }
        else if (_currentQuest is KillQuest killQuest)
        {
            _killPanel.SetActive(true);
            _killController.LoadData(killQuest);
        }
    }

    private void SetDropdownValue()
    {
        if (_currentQuest is TalkQuest)
            _questTypeDropdown.SetValueWithoutNotify(0);
        else if (_currentQuest is FetchQuest)
            _questTypeDropdown.SetValueWithoutNotify(1);
        else if (_currentQuest is KillQuest)
            _questTypeDropdown.SetValueWithoutNotify(2);
    }

    private void OnQuestTypeChanged(int typeIndex)
    {
        if (!_isEditing) return;

        // Create before snapshot
        BaseQuest beforeChange = _currentQuest.Clone();

        // Convert to new type if needed
        switch (typeIndex)
        {
            case 0 when !(_currentQuest is TalkQuest):
                ConvertToNewType<TalkQuest>();
                break;
            case 1 when !(_currentQuest is FetchQuest):
                ConvertToNewType<FetchQuest>();
                break;
            case 2 when !(_currentQuest is KillQuest):
                ConvertToNewType<KillQuest>();
                break;
        }

        // Create and execute command
        var command = new UpdateQuestCommand(beforeChange, _currentQuest);
        CommandManager.Instance.ExecuteCommand(command);

        // Refresh UI
        UpdateActivePanel();
    }

    private void ConvertToNewType<T>() where T : BaseQuest, new()
    {
        // Create new instance of the correct type
        var converted = new T();

        // Copy common properties
        converted.QuestID = _currentQuest.QuestID;
        converted.Title = _currentQuest.Title;
        converted.Description = _currentQuest.Description;
        converted.Steps = new List<QuestStep>(_currentQuest.Steps);

        // Update references
        _currentQuest = converted;
    }

    private void SaveChanges()
    {
        if (_currentQuest == null) return;

        // Create modified version
        BaseQuest modifiedQuest = _currentQuest.Clone();
        modifiedQuest.Title = _titleInput.text;
        modifiedQuest.Description = _descriptionInput.text;

        // Save type-specific data BEFORE updating references
        SaveTypeSpecificData(modifiedQuest);

        // Check if type changed
        bool typeChanged = _currentQuest.GetType() != modifiedQuest.GetType();

        if (typeChanged)
        {
            // Handle type conversion
            QuestManager.Instance.ConvertQuestType(_currentQuest, modifiedQuest);
        }
        else
        {
            // Standard update
            QuestManager.Instance.UpdateQuest(_currentQuest, modifiedQuest);
        }

        // Update current references
        _currentQuest = modifiedQuest;
        _originalState = modifiedQuest.Clone();

        CloseEditor();
    }

    private void SaveTypeSpecificData(BaseQuest quest)
    {
        // Save data based on the ACTUAL type of the quest
        if (quest is TalkQuest talkQuest)
        {
            Debug.Log($"Saving TalkQuest data: {talkQuest.Title}");
            SaveTalkQuestData(talkQuest);
        }
        else if (quest is FetchQuest fetchQuest)
        {
            Debug.Log($"Saving FetchQuest data: {fetchQuest.Title}");
            SaveFetchQuestData(fetchQuest);
        }
        else if (quest is KillQuest killQuest)
        {
            Debug.Log($"Saving KillQuest data: {killQuest.Title}");
            SaveKillQuestData(killQuest);
        }
    }

    // Directly save TalkQuest data from UI
    private void SaveTalkQuestData(TalkQuest quest)
    {
        if (_talkListContainer == null) return;

        quest.NPCTargets.Clear();

        foreach (Transform child in _talkListContainer)
        {
            if (child == null) continue;

            var input = child.GetComponent<TMP_InputField>();
            if (input != null && !string.IsNullOrEmpty(input.text))
            {
                quest.NPCTargets.Add(input.text);
            }
        }
    }

    // Directly save FetchQuest data from UI
    private void SaveFetchQuestData(FetchQuest quest)
    {
        if (_fetchListContainer == null) return;

        quest.RequiredItems.Clear();

        foreach (Transform child in _fetchListContainer)
        {
            if (child == null) continue;

            var fields = child.GetComponentsInChildren<TMP_InputField>();
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

    // Directly save KillQuest data from UI
    private void SaveKillQuestData(KillQuest quest)
    {
        if (_killListContainer == null) return;

        quest.Targets.Clear();

        foreach (Transform child in _killListContainer)
        {
            if (child == null) continue;

            var fields = child.GetComponentsInChildren<TMP_InputField>();
            if (fields.Length >= 2 && !string.IsNullOrEmpty(fields[0].text))
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

    private void OnInputFieldChanged()
    {
        if (!_isEditing) return;

        // Create modified version
        var modifiedQuest = _currentQuest.Clone();
        modifiedQuest.Title = _titleInput.text;
        modifiedQuest.Description = _descriptionInput.text;

        // Create and execute command
        var command = new UpdateQuestCommand(_currentQuest, modifiedQuest);
        CommandManager.Instance.ExecuteCommand(command);

        // Update current references
        _currentQuest = modifiedQuest;
        _originalState = modifiedQuest.Clone();
    }

    private void CloseEditor()
    {
        _isEditing = false;
        _editorPanel.SetActive(false);
        _currentQuest = null;
    }

    private void OnDestroy()
    {
        AppEvents.OnQuestSelected -= OpenEditor;
    }
}