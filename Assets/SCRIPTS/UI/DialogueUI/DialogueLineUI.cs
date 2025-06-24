using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLineUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button deleteButton;

    public string Text => inputField.text;

    public event System.Action<DialogueLineUI> OnDelete;

    private void Awake()
    {
        deleteButton.onClick.AddListener(() => OnDelete?.Invoke(this));
    }

    public void Initialize(string text)
    {
        inputField.text = text;
    }
}