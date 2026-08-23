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

        public bool IsStopCharacter { get; }
        public int GetWaveCount();
    }

    public interface IBattleState
    {
        BattleState CurrentBattleState { get; }
        void ChangeBattleState(BattleState newState);
    }

    public enum BattleState
    {
        WaitingForNextWave,
        WaveInProgress,
        ArenaObjectEditing,
        WaveCompleted,
        WaveFailed
    }

    public class WaveController : MonoBehaviour, IWave, IBattleState
    {
        [SerializeField] WaveConfig waveConfig;
        [Header("テスト")]
        [SerializeField] ArenaObjectData testArenaObjectData;

        IEnemySpawn enemySpawn;
        IEnemyExpFactory enemyExpFactory;
        IPlayerSpawn playerSpawn;
        IExpHandler playerExpHandler;
        IArenaObjectEdit arenaObjectEdit;
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
        public int GetWaveCount() => waveCount;
        public BattleState CurrentBattleState => battleState;
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
            ApiProvider.Register<IBattleState>(this);
            wait_EncountToStartBattle = new WaitForSeconds(waveConfig.EncountToStartBattle);
            wait_TimeAfterWaveCompleted = new WaitForSeconds(waveConfig.TimeAfterWaveCompleted);
            wait_TimeAfterWaveFailed = new WaitForSeconds(waveConfig.TimeAfterWaveFailed);
        }

        void Start()
        {
            enemySpawn = ApiProvider.Get<IEnemySpawn>();
            playerSpawn = ApiProvider.Get<IPlayerSpawn>();
            playerExpHandler = ApiProvider.Get<IExpHandler>();
            arenaObjectEdit = ApiProvider.Get<IArenaObjectEdit>();
            stage = ApiProvider.Get<IStage>();
            cam = ApiProvider.Get<ICamera>();
            enemyExpFactory = ApiProvider.Get<IEnemyExpFactory>();

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

                        stage.CheckCreateWall(spawnPosition);      // Wallを生成する
                        arenaObjectEdit.CreateArenaObjects(testArenaObjectData);    // ArenaObjectを生成する
                        
                        // プレイヤーが敵にエンカウントしたらStateを変える
                        ChangeBattleState(BattleState.WaveInProgress);
                        break;
                    case BattleState.WaveInProgress:
                        // 敵をランダムな位置に複数体生成する
                        int spawnCount = EnemySpawnCount();
                        enemySpawn.OtherSpawnEnemy(spawnCount);

                        yield return wait_EncountToStartBattle;     // プレイヤーと敵が接触してから戦闘が開始するまでの待機時間

                        cam.SetCameraState(CameraState.Battle);
                        IsStopCharacter = false;                    // プレイヤーの入力を許可

                        // ↓全ての敵を倒したら次のwaveへ移行する
                        yield return wait_UntilAllEnemiesDestructed;
                        IsStopCharacter = true;     // プレイヤーの入力を禁止

                        ChangeBattleState(BattleState.WaveCompleted);                       
                        break;
                    case BattleState.WaveCompleted:
                        playerExpHandler.AddPlayerEXP(enemyExpFactory.WaveEXPPool);    // Wave完了後に獲得したEXPをプレイヤーに加算

                        // プレイヤーのレベルアップ回数分、ArenaObjectを選択する
                        yield return arenaObjectEdit.ArenaObjectSelectRoutine(playerExpHandler.LevelUpCount);

                        stage.GetCurrentWall().InactivateWall();      // Wallを非表示

                        // Wave完了後の処理
                        yield return wait_TimeAfterWaveCompleted;
                        playerExpHandler.ResetLevelUpCount();    // Wave完了後にレベルアップ回数をリセット
                        enemyExpFactory.ResetWaveEXPPool();      // Wave完了後にEXPプールをリセット
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