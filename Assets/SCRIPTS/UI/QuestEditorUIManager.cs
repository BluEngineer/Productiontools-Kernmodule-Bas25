using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEditorUIManager : MonoBehaviour
{
    [Header("TMP References")]
    [SerializeField] private GameObject _editorPanel;
    [SerializeField] private TMP_InputField _titleInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private Button _saveButton;

    private BaseQuest _currentQuest;

    void Start()
    {
        _saveButton.onClick.AddListener(SaveChanges);
        AppEvents.OnQuestSelected += OpenEditor;
        _editorPanel.SetActive(false);
    }

    private void OpenEditor(BaseQuest quest)
    {
        _currentQuest = quest;
        _titleInput.text = quest.Title;
        _descriptionInput.text = quest.Description; // Fixed property access
        _editorPanel.SetActive(true);
    }

    private void SaveChanges()
    {
        if (_currentQuest == null) return;

        // Create modified version using Clone()
        BaseQuest modifiedQuest = _currentQuest.Clone();

        // Assign values to properties
        modifiedQuest.Title = _titleInput.text;
        modifiedQuest.Description = _descriptionInput.text;

        // Update through command pattern
        QuestManager.Instance.UpdateQuest(_currentQuest, modifiedQuest);
        CloseEditor();
    }

    private void CloseEditor()
    {
        _editorPanel.SetActive(false);
        _currentQuest = null;
    }
}