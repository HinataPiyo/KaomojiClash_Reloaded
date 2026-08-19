namespace Wave
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "WaveConfig", menuName = "KaomojiClash_Reloaded/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] int minEnemySpawnCount = 2;
        [SerializeField] float increaseSpawnCountPerWave = 1.5f;
        [SerializeField] float timeBetweenWaves = 3f;       // Waveが終了してから次のWaveが開始するまでの待機時間
        [SerializeField] float timeAfterWaveCompleted = 2f;     // Waveが完了した後に次のWaveが開始するまでの待機時間
        [SerializeField] float timeAfterWaveFailed = 2f;        // Waveが失敗した後に次のWaveが開始するまでの待機時間

        public int MinEnemySpawnCount => minEnemySpawnCount;
        public float IncreaseSpawnCountPerWave => increaseSpawnCountPerWave;
        public float TimeBetweenWaves => timeBetweenWaves;
        public float TimeAfterWaveCompleted => timeAfterWaveCompleted;
        public float TimeAfterWaveFailed => timeAfterWaveFailed;
    }

}