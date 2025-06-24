using UnityEngine;

[CreateAssetMenu(menuName = "Quest Entities/NPC", fileName = "NPC_")]
public class NPCData : EntityData
{
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
