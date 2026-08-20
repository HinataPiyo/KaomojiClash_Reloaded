namespace Wave
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "WaveConfig", menuName = "KaomojiClash_Reloaded/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] int minEnemySpawnCount = 2;
        [SerializeField] float increaseSpawnCountPerWave = 1.5f;
        [SerializeField] float encountToStartBattle = 2f;       // プレイヤーと敵が接触してから戦闘が開始するまでの待機時間
        [SerializeField] float timeAfterWaveCompleted = 2f;     // Waveが完了した後に次のWaveが開始するまでの待機時間
        [SerializeField] float timeAfterWaveFailed = 2f;        // Waveが失敗した後に次のWaveが開始するまでの待機時間

        public int MinEnemySpawnCount => minEnemySpawnCount;
        public float IncreaseSpawnCountPerWave => increaseSpawnCountPerWave;
        public float EncountToStartBattle => encountToStartBattle;
        public float TimeAfterWaveCompleted => timeAfterWaveCompleted;
        public float TimeAfterWaveFailed => timeAfterWaveFailed;
    }

}