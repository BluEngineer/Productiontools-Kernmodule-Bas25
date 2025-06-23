using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class QuestItemUI : MonoBehaviour
{
    [Header("Common Components")]
    [SerializeField] protected TMP_Dropdown entityDropdown;
    [SerializeField] protected Button deleteButton;

    protected EntityDatabase entityDB;

    protected virtual void Awake()
    {
        entityDB = EntityDatabase.Instance;
        deleteButton.onClick.AddListener(DeleteItem);
    }

    public abstract void Initialize(string entityId, int amount = 1);

    protected abstract void PopulateDropdown();

    public abstract string GetSelectedEntityId();

    public abstract int GetAmount();

    private void DeleteItem()
    {
        Destroy(gameObject);
    }


}