namespace Enemy
{
    using System.Collections.Generic;
    using UnityEngine;
    using Wall;
    using Wave;

    public interface IEnemySpawn
    {
        void SpawnEnemy();
        bool IsTotalDestructed();
    }

    public class EnemySpawn : MonoBehaviour, IEnemySpawn
    {
        [SerializeField] GameObject enemyPrefab;

        List<GameObject> spawnedEnemies = new List<GameObject>();

        IWall wall;
        IWave wave;

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
            wall = ApiProvider.Get<IWall>();
            wave = ApiProvider.Get<IWave>();
        }

        public void SpawnEnemy()
        {
            Vector2 spawnPosition = wall.GetRandomPositionWithinWall();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            KaomojiData kaomojiData = CreateEnemyKaomojiData();
            enemy.GetComponentInChildren<KaomojiSetUp>().SetUp(kaomojiData);
            spawnedEnemies.Add(enemy);
        }

        /// <summary>
        /// 敵の顔文字データを作成する
        /// </summary>
        /// <param name="releaseStep">Waveが増えるごとにステップが上昇し、装着できるSymbolTypeが増える</param>
        /// <returns></returns>
        KaomojiData CreateEnemyKaomojiData()
        {
            KaomojiData data = ScriptableObject.CreateInstance<KaomojiData>();
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