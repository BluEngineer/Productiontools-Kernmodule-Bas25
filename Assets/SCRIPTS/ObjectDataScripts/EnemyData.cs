using UnityEngine;

[CreateAssetMenu(menuName = "Quest Entities/Enemy", fileName = "ENEMY_")]
public class EnemyData : EntityData
{
    //public string ID;
    //public string DisplayName;
    //[TextArea] public string Description;
    public int CombatLevel = 1;
    public bool IsBoss;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = $"enemy_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}