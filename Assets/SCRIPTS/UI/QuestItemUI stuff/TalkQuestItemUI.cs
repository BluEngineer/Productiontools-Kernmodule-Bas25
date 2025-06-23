public class TalkQuestItemUI : QuestItemUI
{
    private EntityDatabase _entityDB;

    public override void Initialize(string npcId, int amount = 1)
    {
        PopulateDropdown();
        SetDropdownValue(npcId);
    }

    protected override void PopulateDropdown()
    {
        EntityDropdownHelper.PopulateDropdown(entityDropdown, entityDB.NPCs);
    }

    public override string GetSelectedEntityId()
    {
        return EntityDropdownHelper.GetSelectedID(entityDropdown, entityDB.NPCs);
    }

    public override int GetAmount() => 1; // Not applicable for talk items

    private void SetDropdownValue(string npcId)
    {
        EntityDropdownHelper.SetDropdownValue(entityDropdown, npcId, entityDB.NPCs);
    }

}