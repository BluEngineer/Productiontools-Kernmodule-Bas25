using UnityEngine;

[CreateAssetMenu(menuName = "Quest Entities/NPC", fileName = "NPC_")]
public class NPCData : EntityData
{
    public string ID;
    public string DisplayName;
    [TextArea] public string Description;
    public bool CanGiveQuests = true;
    public bool CanReceiveQuests = true;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = $"npc_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}
