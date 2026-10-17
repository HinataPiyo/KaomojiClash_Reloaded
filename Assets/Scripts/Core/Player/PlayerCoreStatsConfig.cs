namespace Player
{
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "PlayerCoreStatsConfig", menuName = "KaomojiClash_Reloaded/PlayerCoreStatsConfig")]
    public class PlayerCoreStatsConfig : CoreStatsConfig
    {
        public float GetDefaultStatusValue(StatusType type)
        {
            if(type == StatusType.CriticalDamage || type == StatusType.CriticalRate)
            {
                Debug.LogWarning($"StatusType {type} does not have a default value defined.");
                return 0f; // or some other default value
            }

            return type switch
            {
                StatusType.Speed => Default_Speed,
                StatusType.Power => Default_Power,
                StatusType.Stamina => Default_Stamina,
                StatusType.Guard => Default_Guard,
                _ => throw new System.ArgumentException($"Invalid StatusType: {type}"),
            };
        }
    }
}