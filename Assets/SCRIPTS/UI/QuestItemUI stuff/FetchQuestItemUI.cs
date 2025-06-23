using TMPro;
using UnityEngine;

public class FetchQuestItemUI : QuestItemUI
{
    [SerializeField] private TMP_InputField amountInput;

    public override void Initialize(string itemId, int amount = 1)
    {
        PopulateDropdown();
        SetDropdownValue(itemId);
        amountInput.text = amount.ToString();
    }

    protected override void PopulateDropdown()
    {
        EntityDropdownHelper.PopulateDropdown(entityDropdown, entityDB.Items);
    }

    public override string GetSelectedEntityId()
    {
        return EntityDropdownHelper.GetSelectedID(entityDropdown, entityDB.Items);
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