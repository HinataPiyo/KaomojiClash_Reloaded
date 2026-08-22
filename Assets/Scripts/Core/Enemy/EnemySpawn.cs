namespace Enemy
{
    using System.Collections.Generic;
    using UnityEngine;
    using Wave;

    public interface IEnemySpawn
    {
        void OnlySpawnEnemy(Vector2 position);
        void OtherSpawnEnemy(int enemyCount);
        bool IsTotalDestructed();
    }

    public class EnemySpawn : MonoBehaviour, IEnemySpawn
    {
        [SerializeField] GameObject enemyPrefab;

        List<GameObject> spawnedEnemies = new List<GameObject>();

        IWave wave;
        IStage stage;
        ICamera cam;

        /// <summary>
        /// 敵が全滅しているかどうかを判定する
        /// </summary>
        public bool IsTotalDestructed() => spawnedEnemies.Count == 0;

        void Awake()
        {
            ApiProvider.Register<IEnemySpawn>(this);
        }

        void Start()
        {
            wave = ApiProvider.Get<IWave>();
            stage = ApiProvider.Get<IStage>();
            cam = ApiProvider.Get<ICamera>();
        }

        void OnEnemyDeath(GameObject enemy)
        {
            spawnedEnemies.Remove(enemy);
            cam.RemoveTargetFromGroup(enemy.transform);
            cam.SetCameraState(CameraState.EnemyDeath);
        }

        /// <summary>
        /// 指定した位置に敵を生成する
        /// </summary>
        /// <param name="position">敵を生成する位置</param>
        public void OnlySpawnEnemy(Vector2 position)
        {
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            IEnemyStamina stamina = enemy.GetComponent<IEnemyStamina>();
            KaomojiData kaomojiData = CreateEnemyKaomojiData();
            stamina.OnEnemyDeathEvent += () => OnEnemyDeath(enemy);
            enemy.GetComponentInChildren<IKaomojiSetUp>().SetUp(kaomojiData);
            spawnedEnemies.Add(enemy);
            cam.AddTargetToGroup(enemy.transform);
        }

        /// <summary>
        /// 敵をランダムな位置に生成する
        /// </summary>
        /// <param name="enemyCount">生成する敵の数</param>
        public void OtherSpawnEnemy(int enemyCount)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Vector2 spawnPosition = stage.GetCurrentWall().GetRandomPositionWithinWall();
                OnlySpawnEnemy(spawnPosition);
            }
        }

        /// <summary>
        /// 敵の顔文字データを作成する
        /// </summary>
        /// <param name="releaseStep">Waveが増えるごとにステップが上昇し、装着できるSymbolTypeが増える</param>
        /// <returns></returns>
        KaomojiData CreateEnemyKaomojiData()
        {
            KaomojiData data = ScriptableObject.CreateInstance<KaomojiData>();
            if(wave == null)
            {
                Debug.LogError("Wave is not initialized.");
                return data;
            }

            for (int i = 0; i < wave.ReleaseStep; i++)
            {
                SymbolType GetSymbolTypeByIndex(int index)
                {
                    switch (index)
                    {
                        case 0: return SymbolType.Mouth;
                        case 1: return SymbolType.LeftEye;
                        case 2: return SymbolType.RightEye;
                        case 3: return SymbolType.LeftHand;
                        case 4: return SymbolType.RightHand;
                        default: throw new System.ArgumentException($"Invalid index for SymbolType: {index}");
                    }
                }

                SymbolType type = GetSymbolTypeByIndex(i);
                SymbolData symbolData;

                // 目だけ両方同じものにする
                if(type == SymbolType.RightEye)
                {
                    symbolData = data.GetSymbolDataByType(SymbolType.LeftEye);
                }
                else
                {
                    symbolData = SymbolDataCollection.GetRandomSymbolData(type);
                }
                
                // 取得したSymbolDataをKaomojiDataにセットする
                data.SetSymbolDataByType(type, symbolData);
            }

            Debug.Log($"Created enemy KaomojiData: {data}");
            return data;
        }
    }
}