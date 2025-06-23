using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillQuestItemUI : QuestItemUI
{
    [SerializeField] private TMP_InputField countInput;
    [SerializeField] private Image entityIcon; // Reference to the Image component

    protected override void Awake()
    {
        base.Awake();
        entityDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }


    public override void Initialize(string enemyId, int count = 1)
    {
        // FIRST: Initialize base components
        PopulateDropdown();
        SetDropdownValue(enemyId);
        countInput.text = count.ToString();  // Or amountInput for Fetch

        // THEN: Update the icon
        UpdateIcon(enemyId);

        // FINALLY: Add listener
        entityDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    protected override void PopulateDropdown()
    {
        EntityDropdownHelper.PopulateDropdown(entityDropdown, entityDB.Enemies);
    }

    public override string GetSelectedEntityId()
    {
        return EntityDropdownHelper.GetSelectedID(entityDropdown, entityDB.Enemies);
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
        return int.TryParse(countInput.text, out int result) ? result : 1;
    }

    private void SetDropdownValue(string enemyId)
    {
        EntityDropdownHelper.SetDropdownValue(entityDropdown, enemyId, entityDB.Enemies);
    }
}