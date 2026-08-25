namespace Enemy
{
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "EnemyCoreStatsConfig", menuName = "KaomojiClash_Reloaded/EnemyCoreStatsConfig")]
    public class EnemyCoreStatsConfig : ScriptableObject
    {
        [Header("コアステータス")]
        [field: SerializeField] public float Default_Stamina = 10f;
        [field: SerializeField] public float Default_Speed = 5f;
        [field: SerializeField] public float Default_Power = 1f;
        [field: SerializeField] public float Default_Defense = 1f;

        [Header("移動関連")]
        [field: SerializeField] public float IdleTime = 1f;                        // 何もしない時間
        [field: SerializeField] public float MaxDraggingDistance = 3f;
        [field: SerializeField] public float DraggingIdleTime = 1f;                // ドラッグ中の待機時間
        [field: SerializeField] public float BeforeLaunchIdleDuration = 0.25f;     // 発射方向が確定したのちの待機時間
    }
}