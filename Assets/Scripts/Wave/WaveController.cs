namespace Wave
{
    using System.Collections;
    using Enemy;
    using UnityEngine;

    public interface IWave
    {
        int EnemySpawnCount();
        int ReleaseStep { get; }

        void ChangeBattleState(BattleState newState);
    }

    public enum BattleState
    {
        WaitingForNextWave,
        WaveInProgress,
        WaveCompleted,
        WaveFailed
    }

    public class WaveController : MonoBehaviour, IWave
    {
        [SerializeField] WaveConfig waveConfig;

        IEnemySpawn enemySpawn;

        BattleState battleState = BattleState.WaitingForNextWave;

        int waveCount = 0;
        int releaseStep = 1;
        WaitForSeconds wait_TimeBetweenWaves;
        WaitForSeconds wait_TimeAfterWaveCompleted;
        WaitForSeconds wait_TimeAfterWaveFailed;
        WaitUntil wait_UntilAllEnemiesDestructed;

        public int ReleaseStep => releaseStep;
        public void ChangeBattleState(BattleState newState) => battleState = newState;

        public int EnemySpawnCount()
        {
            // 1Waveごとに1.5倍ずつ増加し、10Waveごとに敵の生成数をリセットし、Kaomojiの構成を変化させる
            int spawnCount = Mathf.FloorToInt(waveConfig.MinEnemySpawnCount * Mathf.Pow(waveConfig.IncreaseSpawnCountPerWave, waveCount));

            // 10Waveごとに敵の生成数をリセットし、releaseStepを増加させる
            if (waveCount % 10 == 0 && waveCount != 0)
            {
                spawnCount = waveConfig.MinEnemySpawnCount;
                releaseStep++;
            }

            Debug.Log($"Wave {waveCount} - ReleaseStep: {releaseStep}, EnemySpawnCount: {spawnCount}");

            return spawnCount;
        }

        void Awake()
        {
            ApiProvider.Register<IWave>(this);
            wait_TimeBetweenWaves = new WaitForSeconds(waveConfig.TimeBetweenWaves);
            wait_TimeAfterWaveCompleted = new WaitForSeconds(waveConfig.TimeAfterWaveCompleted);
            wait_TimeAfterWaveFailed = new WaitForSeconds(waveConfig.TimeAfterWaveFailed);
        }

        void Start()
        {
            enemySpawn = ApiProvider.Get<IEnemySpawn>();
            wait_UntilAllEnemiesDestructed = new WaitUntil(() => enemySpawn.IsTotalDestructed());
            StartCoroutine(WaveLooping());
        }

        /// <summary>
        /// Waveのループ処理を開始する
        /// </summary>
        IEnumerator WaveLooping()
        {
            while (true)
            {
                switch (battleState)
                {
                    case BattleState.WaitingForNextWave:
                        // 次のWaveを待機
                        yield return wait_TimeBetweenWaves;
                        waveCount++;
                        ChangeBattleState(BattleState.WaveInProgress);
                        break;
                    case BattleState.WaveInProgress:
                        // 敵を生成
                        int spawnCount = EnemySpawnCount();
                        for (int i = 0; i < spawnCount; i++)
                        {
                            enemySpawn.SpawnEnemy();
                        }

                        yield return wait_UntilAllEnemiesDestructed;
                        break;
                    case BattleState.WaveCompleted:
                        // Wave完了後の処理
                        yield return wait_TimeAfterWaveCompleted;
                        ChangeBattleState(BattleState.WaitingForNextWave);
                        break;
                    case BattleState.WaveFailed:
                        // Wave失敗後の処理
                        yield return wait_TimeAfterWaveFailed;
                        yield break;
                }
            }
        }
    }

}