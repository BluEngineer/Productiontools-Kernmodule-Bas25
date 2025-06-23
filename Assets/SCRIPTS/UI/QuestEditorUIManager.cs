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




    private BaseQuest _originalQuest;
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

        // Set reward fields
        _rewardQuantityInput.text = quest.Reward.Quantity.ToString();
        _rewardGoldInput.text = quest.Reward.Gold.ToString();
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


        // Update rewards
        modifiedQuest.Reward.Quantity = ParseInt(_rewardQuantityInput.text);
        modifiedQuest.Reward.Gold = ParseInt(_rewardGoldInput.text);
        //modifiedQuest.Reward.Experience = ParseInt(_rewardExpInput.text);

        // Update type-specific data (using current UI type)
        switch (_questTypeDropdown.value)
        {
            case 0: SaveTalkData((TalkQuest)modifiedQuest); break;
            case 1: SaveFetchData((FetchQuest)modifiedQuest); break;
            case 2: SaveKillData((KillQuest)modifiedQuest); break;
        }

        // Get values from dropdowns
        modifiedQuest.StartNPC = EntityDropdownHelper.GetSelectedID(_startNPCDropdown, _entityDB.NPCs);
        modifiedQuest.DeliveryNPC = EntityDropdownHelper.GetSelectedID(_deliveryNPCDropdown, _entityDB.NPCs);
        modifiedQuest.Reward.ItemID = EntityDropdownHelper.GetSelectedID(_rewardItemDropdown, _entityDB.Items);

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


    private void CloseEditor()
    {
        _isEditing = false;
        _editorPanel.SetActive(false);
        _originalQuest = null;

        _startNPCDropdown.onValueChanged.RemoveAllListeners();
        _deliveryNPCDropdown.onValueChanged.RemoveAllListeners();
        _rewardItemDropdown.onValueChanged.RemoveAllListeners();
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
