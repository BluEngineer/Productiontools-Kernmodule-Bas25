using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;


public class QuestEditorUIManager : MonoBehaviour
{
    [Header("Core UI")]
    [SerializeField] private GameObject _editorPanel;
    [SerializeField] private TMP_InputField _titleInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private Button _saveButton;
    [SerializeField] private TMP_Dropdown _questTypeDropdown;

    [Header("Reward Settings")]
    [SerializeField] private TMP_InputField _rewardQuantityInput;
    [SerializeField] private TMP_InputField _rewardGoldInput;
    [SerializeField] private TMP_InputField _rewardExpInput;
    [SerializeField] private TMP_Dropdown _rewardItemDropdown;

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

    [Header("Entity References")]
    [SerializeField] private TMP_Dropdown _startNPCDropdown;
    [SerializeField] private TMP_Dropdown _deliveryNPCDropdown;

    [Header("Entity Icons")] // Cool little icons for the dropdown menu's
    [SerializeField] private Image _startNPCIcon;
    [SerializeField] private Image _deliveryNPCIcon;
    [SerializeField] private Image _rewardItemIcon;


    [Header("Dialogue Settings")]
    [SerializeField] private GameObject _dialogueLinePrefab;
    [SerializeField] private Transform _startDialogueContainer;
    [SerializeField] private Transform _completionDialogueContainer;
    [SerializeField] private Button _addStartLineButton;
    [SerializeField] private Button _addCompletionLineButton;

    private List<DialogueLineUI> _startLines = new List<DialogueLineUI>();
    private List<DialogueLineUI> _completionLines = new List<DialogueLineUI>();



    private BaseQuest _originalQuest;
    private BaseQuest _workingCopy;
    private bool _isEditing;
    private EntityDatabase _entityDB;

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

        AppEvents.OnCloseQuestEditor += CloseEditor;
        AppEvents.OnRefreshQuestEditor += RefreshEditor;

        AppEvents.OnUndoRedoPerformed += HandleUndoRedo;
        AppEvents.OnQuestAffectedByUndoRedo += HandleAffectedQuest;

        _entityDB = EntityDatabase.Instance;
        PopulateEntityDropdowns();

        _addStartLineButton.onClick.AddListener(() => AddDialogueLine(true));
        _addCompletionLineButton.onClick.AddListener(() => AddDialogueLine(false));

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
        _workingCopy = quest.Clone();

        // Clear existing items
        ClearList(_talkListContainer);
        ClearList(_fetchListContainer);
        ClearList(_killListContainer);

        // Set common fields
        _titleInput.text = _workingCopy.Title;
        _descriptionInput.text = _workingCopy.Description;

        // Set reward fields
        _rewardQuantityInput.text = _workingCopy.Reward.Quantity.ToString();
        _rewardGoldInput.text = _workingCopy.Reward.Gold.ToString();
        //_rewardExpInput.text = quest.Reward.Experience.ToString();

        // Add icon updates
        UpdateEntityIcon(_startNPCIcon, quest.StartNPC, _entityDB.NPCs);
        UpdateEntityIcon(_deliveryNPCIcon, quest.DeliveryNPC, _entityDB.NPCs);
        UpdateEntityIcon(_rewardItemIcon, quest.Reward?.ItemID, _entityDB.Items);

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
        EntityDropdownHelper.SetDropdownValue(_startNPCDropdown, quest.StartNPC, _entityDB.NPCs);
        EntityDropdownHelper.SetDropdownValue(_deliveryNPCDropdown, quest.DeliveryNPC, _entityDB.NPCs);
        EntityDropdownHelper.SetDropdownValue(_rewardItemDropdown, quest.Reward?.ItemID, _entityDB.Items);

        UpdateNPCDropdownIcon(_startNPCIcon, _startNPCDropdown);
        UpdateNPCDropdownIcon(_deliveryNPCIcon, _deliveryNPCDropdown);
        UpdateItemDropdownIcon(_rewardItemIcon, _rewardItemDropdown);

        // Add dropdown listeners
        _startNPCDropdown.onValueChanged.AddListener((index) => UpdateNPCDropdownIcon(_startNPCIcon, _startNPCDropdown));
        _deliveryNPCDropdown.onValueChanged.AddListener((index) => UpdateNPCDropdownIcon(_deliveryNPCIcon, _deliveryNPCDropdown));
        _rewardItemDropdown.onValueChanged.AddListener((index) => UpdateItemDropdownIcon(_rewardItemIcon, _rewardItemDropdown));

        // Clear existing dialogue lines
        ClearDialogueLines(_startDialogueContainer, _startLines);
        ClearDialogueLines(_completionDialogueContainer, _completionLines);

        // Load start dialogue
        foreach (string line in quest.StartDialogueLines)
        {
            AddDialogueLine(true, line);
        }

        // Load completion dialogue
        foreach (string line in quest.CompletionDialogueLines)
        {
            AddDialogueLine(false, line);
        }

        SetupEventListeners();
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
        var dropdown = item.GetComponentInChildren<TMP_Dropdown>();
        EntityDropdownHelper.PopulateDropdown(dropdown, _entityDB.NPCs);
        EntityDropdownHelper.SetDropdownValue(dropdown, npcId, _entityDB.NPCs);
    }


    public void AddFetchItem(string itemId = "", int amount = 1)
    {
        var itemGO = Instantiate(_fetchItemPrefab, _fetchListContainer);
        var dropdown = itemGO.GetComponentInChildren<TMP_Dropdown>();
        var amountInput = itemGO.GetComponentInChildren<TMP_InputField>();

        EntityDropdownHelper.PopulateDropdown(dropdown, _entityDB.Items);
        EntityDropdownHelper.SetDropdownValue(dropdown, itemId, _entityDB.Items);

        if (amountInput) amountInput.text = amount.ToString();
    }

    public void AddKillItem(string enemyId = "", int count = 1)
    {
        var itemGO = Instantiate(_killItemPrefab, _killListContainer);
        var dropdown = itemGO.GetComponentInChildren<TMP_Dropdown>();
        var countInput = itemGO.GetComponentInChildren<TMP_InputField>();

        EntityDropdownHelper.PopulateDropdown(dropdown, _entityDB.Enemies);
        EntityDropdownHelper.SetDropdownValue(dropdown, enemyId, _entityDB.Enemies);

        if (countInput) countInput.text = count.ToString();
    }

    private void PopulateEntityDropdowns()
    {
        EntityDropdownHelper.PopulateDropdown(_startNPCDropdown, _entityDB.NPCs);
        EntityDropdownHelper.PopulateDropdown(_deliveryNPCDropdown, _entityDB.NPCs);
        EntityDropdownHelper.PopulateDropdown(_rewardItemDropdown, _entityDB.Items);
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
        if (_originalQuest == null || _workingCopy == null) return;

        // Determine if quest type has changed
        bool shouldChangeType = (_originalQuest is TalkQuest && _questTypeDropdown.value != 0) ||
                               (_originalQuest is FetchQuest && _questTypeDropdown.value != 1) ||
                               (_originalQuest is KillQuest && _questTypeDropdown.value != 2);

        BaseQuest modifiedQuest = _workingCopy;

        if (shouldChangeType)
        {
            // Create new quest of selected type
            switch (_questTypeDropdown.value)
            {
                case 0: modifiedQuest = new TalkQuest(); break;
                case 1: modifiedQuest = new FetchQuest(); break;
                case 2: modifiedQuest = new KillQuest(); break;
            }

            // Copy common properties
            modifiedQuest.Title = _workingCopy.Title;
            modifiedQuest.Description = _workingCopy.Description;
            modifiedQuest.StartNPC = _workingCopy.StartNPC;
            modifiedQuest.DeliveryNPC = _workingCopy.DeliveryNPC;
            modifiedQuest.Reward = new QuestReward
            {
                ItemID = _workingCopy.Reward.ItemID,
                Quantity = _workingCopy.Reward.Quantity,
                Gold = _workingCopy.Reward.Gold,
                Experience = _workingCopy.Reward.Experience
            };
            modifiedQuest.StartDialogueLines = new List<string>(_workingCopy.StartDialogueLines);
            modifiedQuest.CompletionDialogueLines = new List<string>(_workingCopy.CompletionDialogueLines);
        }

        // Update from UI
        modifiedQuest.Title = _titleInput.text;
        modifiedQuest.Description = _descriptionInput.text;
        modifiedQuest.Reward.Quantity = ParseInt(_rewardQuantityInput.text);
        modifiedQuest.Reward.Gold = ParseInt(_rewardGoldInput.text);

        // Get values from dropdowns
        modifiedQuest.StartNPC = EntityDropdownHelper.GetSelectedID(_startNPCDropdown, _entityDB.NPCs);
        modifiedQuest.DeliveryNPC = EntityDropdownHelper.GetSelectedID(_deliveryNPCDropdown, _entityDB.NPCs);
        modifiedQuest.Reward.ItemID = EntityDropdownHelper.GetSelectedID(_rewardItemDropdown, _entityDB.Items);

        // Update type-specific data with safe casting
        if (modifiedQuest is TalkQuest talkQuest)
        {
            SaveTalkData(talkQuest);
        }
        else if (modifiedQuest is FetchQuest fetchQuest)
        {
            SaveFetchData(fetchQuest);
        }
        else if (modifiedQuest is KillQuest killQuest)
        {
            SaveKillData(killQuest);
        }

        SaveDialogueData(modifiedQuest);

        // Create final update command
        var updateCommand = new UpdateQuestCommand(_originalQuest, modifiedQuest);
        CommandManager.Instance.ExecuteCommand(updateCommand);

        CloseEditor();
    }

    private void SaveTalkData(TalkQuest quest)
    {
        quest.NPCTargets.Clear();
        foreach (Transform child in _talkListContainer)
        {
            // Use GetComponentInChildren to find the UI component
            var item = child.GetComponentInChildren<TalkQuestItemUI>();
            if (item != null)
            {
                string npcId = item.GetSelectedEntityId();
                if (!string.IsNullOrEmpty(npcId))
                    quest.NPCTargets.Add(npcId);
            }
        }
    }

    private void SaveFetchData(FetchQuest quest)
    {
        quest.RequiredItems.Clear();
        foreach (Transform child in _fetchListContainer)
        {
            // Use GetComponentInChildren to find the UI component
            var item = child.GetComponentInChildren<FetchQuestItemUI>();
            if (item != null)
            {
                string itemId = item.GetSelectedEntityId();
                if (!string.IsNullOrEmpty(itemId))
                {
                    quest.RequiredItems.Add(new ItemRequirement
                    {
                        ItemID = itemId,
                        Amount = item.GetAmount()
                    });
                }
            }
        }
    }

    private void SaveKillData(KillQuest quest)
    {
        {
            quest.Targets.Clear();
            foreach (Transform child in _killListContainer)
            {
                // Use GetComponentInChildren to find the UI component
                var item = child.GetComponentInChildren<KillQuestItemUI>();
                if (item != null)
                {
                    string enemyId = item.GetSelectedEntityId();
                    if (!string.IsNullOrEmpty(enemyId))
                    {
                        quest.Targets.Add(new EnemyTarget
                        {
                            EnemyID = enemyId,
                            Count = item.GetAmount()
                        });
                    }
                }
            }
        }
    }

    private void UpdateEntityIcon<T>(Image icon, string entityId, List<T> entityList) where T : EntityData
    {
        if (icon == null) return;

        Sprite iconSprite = null;

        if (!string.IsNullOrEmpty(entityId))
        {
            T entity = entityList.Find(e => e.ID == entityId);
            if (entity != null)
            {
                iconSprite = entity.Icon;
            }
        }

        // Always set the sprite - keeps component enabled
        icon.sprite = iconSprite;
    }

    private void UpdateNPCDropdownIcon(Image icon, TMP_Dropdown dropdown)
    {
        if (icon == null || dropdown == null) return;

        // Always ensure the icon is enabled
        icon.enabled = true;

        string id = EntityDropdownHelper.GetSelectedID(dropdown, _entityDB.NPCs);
        UpdateEntityIcon(icon, id, _entityDB.NPCs);
    }

    private void UpdateItemDropdownIcon(Image icon, TMP_Dropdown dropdown)
    {
        if (icon == null || dropdown == null) return;

        // Always ensure the icon is enabled
        icon.enabled = true;

        string id = EntityDropdownHelper.GetSelectedID(dropdown, _entityDB.Items);
        UpdateEntityIcon(icon, id, _entityDB.Items);
    }
    //DIALOGUE STUFF
    private void AddDialogueLine(bool isStartDialogue, string text = "")
    {
        Transform container = isStartDialogue ? _startDialogueContainer : _completionDialogueContainer;
        List<DialogueLineUI> list = isStartDialogue ? _startLines : _completionLines;

        var lineObj = Instantiate(_dialogueLinePrefab, container);
        var dialogueLine = lineObj.GetComponent<DialogueLineUI>();
        dialogueLine.Initialize(text);

        dialogueLine.OnDelete += (line) => DeleteDialogueLine(line, list);
        list.Add(dialogueLine);
    }

    private void DeleteDialogueLine(DialogueLineUI line, List<DialogueLineUI> list)
    {
        list.Remove(line);
        Destroy(line.gameObject);
    }

    private void ClearDialogueLines(Transform container, List<DialogueLineUI> list)
    {
        foreach (var line in list)
        {
            Destroy(line.gameObject);
        }
        list.Clear();
    }

    private void SaveDialogueData(BaseQuest quest)
    {
        quest.StartDialogueLines.Clear();
        foreach (var line in _startLines)
        {
            quest.StartDialogueLines.Add(line.Text);
        }

        quest.CompletionDialogueLines.Clear();
        foreach (var line in _completionLines)
        {
            quest.CompletionDialogueLines.Add(line.Text);
        }
    }

    private void SetupEventListeners()
    {
        _titleInput.onEndEdit.AddListener(value => CreateFieldCommand(
            () => _workingCopy.Title = value,
            () => _workingCopy.Title = _titleInput.text,
            "Title"
        ));

        _descriptionInput.onEndEdit.AddListener(value => CreateFieldCommand(
            () => _workingCopy.Description = value,
            () => _workingCopy.Description = _descriptionInput.text,
            "Description"
        ));

        // Add similar handlers for other fields:
        _rewardQuantityInput.onEndEdit.AddListener(value => CreateFieldCommand(
            () => _workingCopy.Reward.Quantity = int.TryParse(value, out int result) ? result : 0,
            () => _rewardQuantityInput.text = _workingCopy.Reward.Quantity.ToString(),
            "Reward Quantity"
        ));

        _rewardGoldInput.onEndEdit.AddListener(value => CreateFieldCommand(
            () => _workingCopy.Reward.Gold = int.TryParse(value, out int result) ? result : 0,
            () => _rewardGoldInput.text = _workingCopy.Reward.Gold.ToString(),
            "Gold Reward"
        ));

        // Add dropdown listeners
        _startNPCDropdown.onValueChanged.AddListener(index => {
            string selectedId = EntityDropdownHelper.GetSelectedID(_startNPCDropdown, _entityDB.NPCs);
            CreateFieldCommand(
                () => _workingCopy.StartNPC = selectedId,
                () => EntityDropdownHelper.SetDropdownValue(_startNPCDropdown, _workingCopy.StartNPC, _entityDB.NPCs),
                "Start NPC"
            );
            UpdateNPCDropdownIcon(_startNPCIcon, _startNPCDropdown);
        });

        // Add similar for other dropdowns...
    }

    private void CreateFieldCommand(Action execute, Action undo, string fieldName)
    {
        var command = new FieldEditCommand(
            execute: execute,
            undo: undo,
            fieldName: fieldName,
            quest: _workingCopy
        );

        CommandManager.Instance.ExecuteCommand(command);
    }

private void CloseEditor()
    {
        _isEditing = false;
        _editorPanel.SetActive(false);
        _originalQuest = null;

        _startNPCDropdown.onValueChanged.RemoveAllListeners();
        _deliveryNPCDropdown.onValueChanged.RemoveAllListeners();
        _rewardItemDropdown.onValueChanged.RemoveAllListeners();
    }

    private void InitializeUIWithWorkingCopy()
    {
        if (_workingCopy == null) return;

        _titleInput.text = _workingCopy.Title;
        _descriptionInput.text = _workingCopy.Description;
        _rewardQuantityInput.text = _workingCopy.Reward.Quantity.ToString();
        _rewardGoldInput.text = _workingCopy.Reward.Gold.ToString();

        // Initialize dropdowns
        EntityDropdownHelper.SetDropdownValue(_startNPCDropdown, _workingCopy.StartNPC, _entityDB.NPCs);
        EntityDropdownHelper.SetDropdownValue(_deliveryNPCDropdown, _workingCopy.DeliveryNPC, _entityDB.NPCs);
        EntityDropdownHelper.SetDropdownValue(_rewardItemDropdown, _workingCopy.Reward?.ItemID, _entityDB.Items);

        // Update icons
        UpdateNPCDropdownIcon(_startNPCIcon, _startNPCDropdown);
        UpdateNPCDropdownIcon(_deliveryNPCIcon, _deliveryNPCDropdown);
        UpdateItemDropdownIcon(_rewardItemIcon, _rewardItemDropdown);
    }

    private void RefreshEditor(BaseQuest quest)
    {
        // Only refresh if we're editing this specific quest
        //if (_isEditing && _originalQuest?.ID == quest.ID)
        {
            CloseEditor();
            OpenEditor(quest);
        }
    }

    private void HandleUndoRedo()
    {
        // Always close editor on any undo/redo
        CloseEditor();
    }

    private void HandleAffectedQuest(BaseQuest quest)
    {
        // Optional: Special handling if needed
        // Currently we just close editor for simplicity
    }

    private void OnDestroy()
    {
        AppEvents.OnQuestSelected -= OpenEditor;
        AppEvents.OnCloseQuestEditor -= CloseEditor;
        AppEvents.OnRefreshQuestEditor -= RefreshEditor;
        AppEvents.OnUndoRedoPerformed -= HandleUndoRedo;
        AppEvents.OnQuestAffectedByUndoRedo -= HandleAffectedQuest;
    }



    //this shit should move to a dedicated tools class. wrong responsibility here but we ball
    private int ParseInt(string value)
    {
        return int.TryParse(value, out int result) ? result : 0;
    }
}
