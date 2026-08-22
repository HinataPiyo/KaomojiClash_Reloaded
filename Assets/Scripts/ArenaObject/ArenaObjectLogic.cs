using UnityEngine;

public abstract class ArenaObjectLogic : MonoBehaviour
{
    public int Level { get; private set; } = 1;
    public event System.Action<int> OnLevelChanged;
    public void IncrimentLevel()
    {
        if(Level >= ArenaObjectData.MaxLevel)
        {
            Debug.LogWarning($"<color=yellow>Already at max level: {Level}</color>");
            return;
        }
        
        Level++;
        OnLevelChanged?.Invoke(Level);
    }
}

public abstract class ArenaObjectLogic<TData> : ArenaObjectLogic where TData : ArenaObjectData
{
    [SerializeField] protected TData data;

    public TData Data => data;
}