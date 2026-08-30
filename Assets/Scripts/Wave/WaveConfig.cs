namespace Wave
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "WaveConfig", menuName = "KaomojiClash_Reloaded/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] int minEnemySpawnCount = 2;
        [SerializeField] float increaseSpawnCountPerWave = 1.5f;
        [SerializeField] float encountAnimationTime = 2f;       // プレイヤーと敵が接触してから戦闘が開始するまでの待機時間
        [SerializeField] float startWaveAnimationTime = 2f;        // Wave開始のアニメーションを表示する時間
        [SerializeField] float timeAfterWaveCompleted = 2f;     // Waveが完了した後に次のWaveが開始するまでの待機時間
        [SerializeField] float timeAfterWaveFailed = 2f;        // Waveが失敗した後に次のWaveが開始するまでの待機時間

        public int MinEnemySpawnCount => minEnemySpawnCount;
        public float IncreaseSpawnCountPerWave => increaseSpawnCountPerWave;
        public float EncountAnimationTime => encountAnimationTime;
        public float StartWaveAnimationTime => startWaveAnimationTime;
        public float TimeAfterWaveCompleted => timeAfterWaveCompleted;
        public float TimeAfterWaveFailed => timeAfterWaveFailed;
    }

}