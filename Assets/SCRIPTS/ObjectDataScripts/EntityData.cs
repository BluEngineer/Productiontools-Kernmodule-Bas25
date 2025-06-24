using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    public string ID;

    public string DisplayName;

    [TextArea] public string Description;

    public Sprite Icon;

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = $"{GetType().Name.ToLower()}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}
