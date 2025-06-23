using TMPro;
using UnityEngine;

public class KillQuestItemUI : QuestItemUI
{
    [SerializeField] private TMP_InputField countInput;

    public override void Initialize(string enemyId, int count = 1)
    {
        PopulateDropdown();
        SetDropdownValue(enemyId);
        countInput.text = count.ToString();
    }

    protected override void PopulateDropdown()
    {
        EntityDropdownHelper.PopulateDropdown(entityDropdown, entityDB.Enemies);
    }

    public override string GetSelectedEntityId()
    {
        return EntityDropdownHelper.GetSelectedID(entityDropdown, entityDB.Enemies);
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