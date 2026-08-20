namespace Wave
{
    using System.Collections;
    using Enemy;
    using Player;
    using UnityEngine;

    public interface IWave
    {
        int EnemySpawnCount();
        int ReleaseStep { get; }

        void ChangeBattleState(BattleState newState);
        public bool IsStopCharacter { get; }
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
        IPlayerSpawn playerSpawn;
        IStage stage;
        ICamera cam;

        BattleState battleState = BattleState.WaitingForNextWave;

        int waveCount = 0;
        int releaseStep = 1;
        WaitForSeconds wait_EncountToStartBattle;
        WaitForSeconds wait_TimeAfterWaveCompleted;
        WaitForSeconds wait_TimeAfterWaveFailed;
        WaitUntil wait_UntilAllEnemiesDestructed;
        WaitUntil wait_MovePlayerToEnemy;

        public bool IsStopCharacter { get; private set; } = true;
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
            wait_EncountToStartBattle = new WaitForSeconds(waveConfig.EncountToStartBattle);
            wait_TimeAfterWaveCompleted = new WaitForSeconds(waveConfig.TimeAfterWaveCompleted);
            wait_TimeAfterWaveFailed = new WaitForSeconds(waveConfig.TimeAfterWaveFailed);
        }

        void Start()
        {
            enemySpawn = ApiProvider.Get<IEnemySpawn>();
            playerSpawn = ApiProvider.Get<IPlayerSpawn>();
            stage = ApiProvider.Get<IStage>();
            cam = ApiProvider.Get<ICamera>();

            wait_UntilAllEnemiesDestructed = new WaitUntil(() => enemySpawn.IsTotalDestructed());
            wait_MovePlayerToEnemy = new WaitUntil(() => !playerSpawn.IsMoveToEnemy);
            StartCoroutine(WaveLooping());
        }

        /// <summary>
        /// Waveのループ処理を開始する
        /// </summary>
        IEnumerator WaveLooping()
        {
            yield return null;

            while (true)
            {
                switch (battleState)
                {
                    case BattleState.WaitingForNextWave:
                        IsStopCharacter = true;      // プレイヤーの入力を禁止
                        waveCount++;

                        cam.SetCameraState(CameraState.PlayerMoving);
                        Vector2 spawnPosition = stage.EncountPosition(waveCount);
                        enemySpawn.OnlySpawnEnemy(spawnPosition);       // 最初は一体生成する

                        // プレイヤーが移動し敵とエンカウントしたらその敵を中心にWallを生成する
                        playerSpawn.MovementPlayerToEnemy(spawnPosition);
                        
                        yield return wait_MovePlayerToEnemy;        // プレイヤーが敵の位置に移動するのを待つ

                        // Wallを生成
                        stage.CreateWall(spawnPosition);

                        // プレイヤーが敵にエンカウントしたらStateを変える
                        ChangeBattleState(BattleState.WaveInProgress);
                        break;
                    case BattleState.WaveInProgress:
                        // 敵をランダムな位置に複数体生成する
                        int spawnCount = EnemySpawnCount();
                        enemySpawn.OtherSpawnEnemy(spawnCount);

                        yield return wait_EncountToStartBattle;     // プレイヤーと敵が接触してから戦闘が開始するまでの待機時間

                        cam.SetCameraState(CameraState.Battle);
                        IsStopCharacter = false;     // プレイヤーの入力を許可

                        // ↓全ての敵を倒したら次のwaveへ移行する
                        yield return wait_UntilAllEnemiesDestructed;

                        IsStopCharacter = true;     // プレイヤーの入力を禁止
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