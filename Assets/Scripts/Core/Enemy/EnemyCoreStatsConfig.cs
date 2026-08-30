namespace Enemy
{
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "EnemyCoreStatsConfig", menuName = "KaomojiClash_Reloaded/EnemyCoreStatsConfig")]
    public class EnemyCoreStatsConfig : CoreStatsConfig
    {
        [Header("ドロップ関連")]
        [SerializeField] public int DropMoney = 10; // 敵が倒されたときにドロップするお金の量
        
        [Header("移動関連")]
        [field: SerializeField] public float IdleTime = 1f;                        // 何もしない時間
        [field: SerializeField] public float DraggingIdleTime = 1f;                // ドラッグ中の待機時間
        [field: SerializeField] public float BeforeLaunchIdleDuration = 0.25f;     // 発射方向が確定したのちの待機時間
    }
}