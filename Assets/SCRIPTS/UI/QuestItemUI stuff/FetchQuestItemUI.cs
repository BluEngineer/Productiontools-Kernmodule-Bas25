using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FetchQuestItemUI : QuestItemUI
{
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private Image entityIcon; // Reference to the Image component

    // Add listener in Awake
    protected override void Awake()
    {
        base.Awake();
        entityDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public override void Initialize(string itemId, int amount = 1)
    {
        // FIRST: Initialize base components
        PopulateDropdown();
        SetDropdownValue(itemId);
        amountInput.text = amount.ToString();  // Or amountInput for Fetch

        // THEN: Update the icon
        UpdateIcon(itemId);

        // FINALLY: Add listener
        entityDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    protected override void PopulateDropdown()
    {
        EntityDropdownHelper.PopulateDropdown(entityDropdown, entityDB.Items);
    }

    public override string GetSelectedEntityId()
    {
        return EntityDropdownHelper.GetSelectedID(entityDropdown, entityDB.Items);
    }

    private void OnDropdownChanged(int index)
    {
        string selectedId = GetSelectedEntityId();
        UpdateIcon(selectedId);
    }

    private void UpdateIcon(string entityId)
    {
        if (entityIcon == null)
        {
            Debug.LogError("EntityIcon reference is null!", gameObject);
            return;
        }

        // Always reset to visible state
        entityIcon.enabled = true;

        // Clear sprite if no entity ID
        if (string.IsNullOrEmpty(entityId))
        {
            entityIcon.sprite = null;
            return;
        }

        // Get the entity data DIRECTLY from the database
        EntityData entity = null;

        if (this is FetchQuestItemUI)
            entity = entityDB.GetItemByID(entityId);
        else if (this is KillQuestItemUI)
            entity = entityDB.GetEnemyByID(entityId);
        else if (this is TalkQuestItemUI)
            entity = entityDB.GetNPCByID(entityId);

        // Verify we found the entity
        if (entity == null)
        {
            Debug.LogError($"Entity not found: {entityId}", this);
            entityIcon.sprite = null;
            return;
        }

        // DIRECTLY assign the sprite from the entity
        entityIcon.sprite = entity.Icon;

        // Log the assignment for debugging
        Debug.Log($"Assigned sprite: {(entityIcon.sprite != null ? entityIcon.sprite.name : "NULL")} " +
                  $"to {gameObject.name} for entity {entity.DisplayName}");
    }

    public override int GetAmount()
    {
        return int.TryParse(amountInput.text, out int result) ? result : 1;
    }

    private void SetDropdownValue(string itemId)
    {
        EntityDropdownHelper.SetDropdownValue(entityDropdown, itemId, entityDB.Items);
    }
}