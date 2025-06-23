using UnityEngine;
using UnityEngine.UI;
public class TalkQuestItemUI : QuestItemUI
{
    private EntityDatabase _entityDB;
    [SerializeField] private Image entityIcon; // Reference to the Image component

    protected override void Awake()
    {
        base.Awake();
        entityDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }


    public override void Initialize(string npcId, int amount = 1)
    {
        // FIRST: Initialize base components
        PopulateDropdown();
        SetDropdownValue(npcId);
        //countInput.text = amount.ToString();  // Or amountInput for Fetch

        // THEN: Update the icon
        UpdateIcon(npcId);

        // FINALLY: Add listener
        entityDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    protected override void PopulateDropdown()
    {
        EntityDropdownHelper.PopulateDropdown(entityDropdown, entityDB.NPCs);
    }

    public override string GetSelectedEntityId()
    {
        return EntityDropdownHelper.GetSelectedID(entityDropdown, entityDB.NPCs);
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

    public override int GetAmount() => 1; // Not applicable for talk items

    private void SetDropdownValue(string npcId)
    {
        EntityDropdownHelper.SetDropdownValue(entityDropdown, npcId, entityDB.NPCs);
    }

}