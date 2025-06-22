using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillQuestPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _targetsContainer;
    [SerializeField] private GameObject _targetPrefab;
    [SerializeField] private Button _addTargetButton;

    private KillQuest _currentQuest;

    void Start()
    {
        _addTargetButton.onClick.AddListener(OnAddTarget);
    }

    public void LoadData(KillQuest quest)
    {
        _currentQuest = quest;
        ClearList();

        foreach (var target in quest.Targets)
        {
            AddTarget(target.EnemyID, target.Count);
        }
    }

    public void SaveData(KillQuest quest)
    {
        quest.Targets.Clear();

        foreach (Transform item in _targetsContainer)
        {
            // Skip the add button
            if (item.gameObject == _addTargetButton.gameObject) continue;

            var fields = item.GetComponentsInChildren<TMP_InputField>();
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

    private void AddTarget(string enemyId = "", int count = 1)
    {
        var item = Instantiate(_targetPrefab, _targetsContainer);
        var fields = item.GetComponentsInChildren<TMP_InputField>();

        if (fields.Length >= 2)
        {
            fields[0].text = enemyId;
            fields[1].text = count.ToString();

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
        for (int i = _targetsContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = _targetsContainer.GetChild(i);
            if (child.gameObject != _addTargetButton.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void OnAddTarget()
    {
        BaseQuest beforeChange = _currentQuest.Clone();
        AddTarget();
        SaveData(_currentQuest);
        var command = new UpdateQuestCommand(beforeChange, _currentQuest);
        CommandManager.Instance.ExecuteCommand(command);
    }
}