using UnityEngine;

[CreateAssetMenu(menuName = "Quest Entities/Item", fileName = "ITEM_")]
public class ItemData : EntityData
{
   // public string ID;
   // public string DisplayName;
   // [TextArea] public string Description;
    public bool IsQuestItem;
    public int MaxStack = 1;

    protected override void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = $"item_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}