using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkQuestPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _npcListContainer;
    [SerializeField] private GameObject _npcItemPrefab;
    [SerializeField] private Button _addNPCButton;

    private TalkQuest _currentQuest;

    void Start()
    {
        if (_addNPCButton != null)
            _addNPCButton.onClick.AddListener(OnAddNPC);
    }

    public void LoadData(TalkQuest quest)
    {
        if (quest == null) return;

        _currentQuest = quest;
        ClearList();

        foreach (string npcId in quest.NPCTargets)
        {
            AddNPCItem(npcId);
        }
    }

    public void SaveData(TalkQuest quest)
    {
        if (quest == null || _npcListContainer == null) return;

        quest.NPCTargets.Clear();

        foreach (Transform item in _npcListContainer)
        {
            if (item == null) continue;

            var input = item.GetComponent<TMP_InputField>();
            if (input != null && !string.IsNullOrEmpty(input.text))
            {
                quest.NPCTargets.Add(input.text);
            }
        }
    }

    private void AddNPCItem(string npcId = "")
    {
        if (_npcListContainer == null || _npcItemPrefab == null) return;

        var item = Instantiate(_npcItemPrefab, _npcListContainer);
        var input = item.GetComponent<TMP_InputField>();
        if (input != null) input.text = npcId;

        // Delete button functionality
        var deleteButton = item.GetComponentInChildren<Button>();
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(() => {
                if (_currentQuest == null) return;

                BaseQuest beforeChange = _currentQuest.Clone();
                Destroy(item.gameObject);
                SaveData(_currentQuest);
                var command = new UpdateQuestCommand(beforeChange, _currentQuest);
                CommandManager.Instance.ExecuteCommand(command);
            });
        }

        // Change listener
        if (input != null)
        {
            input.onEndEdit.AddListener((value) => {
                if (_currentQuest == null) return;

                BaseQuest beforeChange = _currentQuest.Clone();
                SaveData(_currentQuest);
                var command = new UpdateQuestCommand(beforeChange, _currentQuest);
                CommandManager.Instance.ExecuteCommand(command);
            });
        }
    }

    public void ClearList()
    {
        if (_npcListContainer == null) return;

        foreach (Transform child in _npcListContainer)
        {
            if (child != null && child.gameObject != _addNPCButton?.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void OnAddNPC()
    {
        if (_currentQuest == null) return;

        BaseQuest beforeChange = _currentQuest.Clone();
        AddNPCItem();
        SaveData(_currentQuest);
        var command = new UpdateQuestCommand(beforeChange, _currentQuest);
        CommandManager.Instance.ExecuteCommand(command);
    }
}