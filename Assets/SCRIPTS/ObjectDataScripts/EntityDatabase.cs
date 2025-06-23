using System.Collections.Generic;
using UnityEngine;

public class EntityDatabase : MonoBehaviour
{
    private static EntityDatabase _instance;
    public static EntityDatabase Instance => _instance ? _instance : FindObjectOfType<EntityDatabase>();

    [Header("Loaded Entities")]
    public List<NPCData> NPCs = new List<NPCData>();
    public List<ItemData> Items = new List<ItemData>();
    public List<EnemyData> Enemies = new List<EnemyData>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllEntities();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllEntities()
    {
        NPCs.AddRange(Resources.LoadAll<NPCData>("Entities/NPCs"));
        Items.AddRange(Resources.LoadAll<ItemData>("Entities/Items"));
        Enemies.AddRange(Resources.LoadAll<EnemyData>("Entities/Enemies"));

        Debug.Log($"Loaded {NPCs.Count} NPCs, {Items.Count} Items, {Enemies.Count} Enemies");
    }

    // Helper method to find entity by ID
    public NPCData GetNPCByID(string id) => NPCs.Find(n => n.ID == id);
    public ItemData GetItemByID(string id) => Items.Find(i => i.ID == id);
    public EnemyData GetEnemyByID(string id) => Enemies.Find(e => e.ID == id);
}