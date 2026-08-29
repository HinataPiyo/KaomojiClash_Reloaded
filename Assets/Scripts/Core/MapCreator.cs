namespace Map
{

    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Pool;
    using Player;
    using Wave;

    public class MapCreator : MonoBehaviour
    {
        [Header("生成するASCIIオブジェクトのテンプレート")]
        [SerializeField] GameObject asciiObjectPrefab;

        [Header("配置候補のASCIIアート")]
        [SerializeField] string[] asciiArts;

        [Header("ASCIIランダム分割設定")]
        [SerializeField] bool useRandomSplit = true;
        [SerializeField, Min(1)] int minSplitLength = 2;

        [Header("生成設定")]
        [SerializeField, Min(1)] int createCount = 100;
        [SerializeField] Vector2 spawnArea = new Vector2(16f, 10f);
        [SerializeField, Min(0.1f)] float objectRadius = 0.5f;
        [SerializeField, Min(1)] int maxTryPerObject = 30;
        [SerializeField] LayerMask blockLayerMask = ~0;
        [SerializeField] bool createOnStart = true;

        // プール管理とチャンク管理用のデータ
        private ObjectPool<GameObject> asciiPool;
        
        class Chunk
        {
            public int index;
            public List<GameObject> activeObjects = new List<GameObject>();
        }

        readonly Dictionary<int, Chunk> activeChunks = new Dictionary<int, Chunk>();
        IPlayerSpawn playerSpawn;
        bool isBattleActive = false;

        void Awake()
        {
            if (Application.isPlaying)
            {
                int defaultCapacity = createCount * 3;
                int maxPoolSize = createCount * 10;
                asciiPool = new ObjectPool<GameObject>(
                    createFunc: OnCreatePooledItem,
                    actionOnGet: OnGetPooledItem,
                    actionOnRelease: OnReleasePooledItem,
                    actionOnDestroy: OnDestroyPooledItem,
                    collectionCheck: true,
                    defaultCapacity: defaultCapacity,
                    maxSize: maxPoolSize
                );
            }
        }

        void Start()
        {
            playerSpawn = ApiProvider.Get<IPlayerSpawn>();

            if (createOnStart)
            {
                UpdateChunks();
            }

            WaveController.OnWaveInProgress += OnBattle;
            WaveController.OnWaitingForNextWave += OutBattle;
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                UpdateChunks();
            }
        }

        void OnDestroy()
        {
            WaveController.OnWaveInProgress -= OnBattle;
            WaveController.OnWaitingForNextWave -= OutBattle;
            
            if (asciiPool != null)
            {
                asciiPool.Clear();
            }
        }

        private GameObject OnCreatePooledItem()
        {
            GameObject obj = Instantiate(asciiObjectPrefab, Vector3.zero, Quaternion.identity, transform);
            return obj;
        }

        private void OnGetPooledItem(GameObject obj)
        {
            obj.SetActive(true);
        }

        private void OnReleasePooledItem(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void OnDestroyPooledItem(GameObject obj)
        {
            Destroy(obj);
        }

        void UpdateChunks()
        {
            float playerX = 0f;
            if (playerSpawn != null && playerSpawn.PlayerInstance != null)
            {
                playerX = playerSpawn.PlayerInstance.transform.position.x;
            }
            else if (Camera.main != null)
            {
                playerX = Camera.main.transform.position.x;
            }

            int currentChunkIndex = Mathf.FloorToInt((playerX - transform.position.x) / spawnArea.x);

            // プレイヤーが右に進んでいくため、表示が途切れないように、
            // 左側に1つ、自チャンク、右側に2つの計4チャンクをアクティブにする
            int startChunk = currentChunkIndex - 1;
            int endChunk = currentChunkIndex + 2;

            // 必要なチャンクの生成
            for (int i = startChunk; i <= endChunk; i++)
            {
                if (!activeChunks.ContainsKey(i))
                {
                    CreateChunk(i);
                }
            }

            // 不要になった古いチャンクの回収
            List<int> keysToRemove = new List<int>();
            foreach (var kvp in activeChunks)
            {
                if (kvp.Key < startChunk || kvp.Key > endChunk)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            for (int i = 0; i < keysToRemove.Count; i++)
            {
                ReleaseChunk(keysToRemove[i]);
            }
        }

        void CreateChunk(int chunkIndex)
        {
            Chunk chunk = new Chunk { index = chunkIndex };
            List<Vector2> tempUsedPositions = new List<Vector2>();

            for (int i = 0; i < createCount; i++)
            {
                if (!TryFindSpawnPositionForChunk(chunkIndex, out Vector2 spawnPos, tempUsedPositions))
                {
                    break;
                }

                GameObject obj = GetAsciiObject(spawnPos);
                string asciiText = CreateRandomAsciiFromArray();
                ApplyAsciiText(obj, asciiText);
                SetObjectColor(obj, isBattleActive ? 0.3f : 1.0f);

                chunk.activeObjects.Add(obj);
                tempUsedPositions.Add(spawnPos);
            }

            activeChunks.Add(chunkIndex, chunk);
        }

        void ReleaseChunk(int chunkIndex)
        {
            if (activeChunks.TryGetValue(chunkIndex, out Chunk chunk))
            {
                for (int i = 0; i < chunk.activeObjects.Count; i++)
                {
                    if (chunk.activeObjects[i] != null)
                    {
                        ReleaseAsciiObject(chunk.activeObjects[i]);
                    }
                }
                activeChunks.Remove(chunkIndex);
            }
        }

        GameObject GetAsciiObject(Vector2 position)
        {
            if (Application.isPlaying)
            {
                GameObject obj = asciiPool.Get();
                obj.transform.position = position;
                obj.transform.rotation = Quaternion.identity;
                return obj;
            }
            else
            {
                return Instantiate(asciiObjectPrefab, position, Quaternion.identity, transform);
            }
        }

        void ReleaseAsciiObject(GameObject obj)
        {
            if (Application.isPlaying)
            {
                if (asciiPool != null)
                {
                    asciiPool.Release(obj);
                }
                else
                {
                    Destroy(obj);
                }
            }
            else
            {
                DestroyImmediate(obj);
            }
        }

        [ContextMenu("Create Map Objects")]
        public void CreateMapObjects()
        {
            if (asciiObjectPrefab == null)
            {
                Debug.LogWarning("MapCreator: asciiObjectPrefab が未設定です。", this);
                return;
            }

            if (asciiArts == null || asciiArts.Length == 0)
            {
                Debug.LogWarning("MapCreator: asciiArts が未設定です。", this);
                return;
            }

            ClearMapObjects();

            if (Application.isPlaying && asciiPool == null)
            {
                Awake();
            }

            CreateChunk(0);
        }

        [ContextMenu("Clear Map Objects")]
        public void ClearMapObjects()
        {
            List<int> keys = new List<int>(activeChunks.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                ReleaseChunk(keys[i]);
            }
            activeChunks.Clear();

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        bool TryFindSpawnPositionForChunk(int chunkIndex, out Vector2 spawnPos, List<Vector2> tempUsedPositions)
        {
            float chunkCenterX = transform.position.x + chunkIndex * spawnArea.x;
            for (int tryCount = 0; tryCount < maxTryPerObject; tryCount++)
            {
                float x = Random.Range(-spawnArea.x * 0.5f, spawnArea.x * 0.5f);
                float y = Random.Range(-spawnArea.y * 0.5f, spawnArea.y * 0.5f);
                Vector2 candidate = new Vector2(chunkCenterX + x, transform.position.y + y);

                if (IsOverlappingForChunk(candidate, tempUsedPositions))
                {
                    continue;
                }

                spawnPos = candidate;
                return true;
            }

            spawnPos = default;
            return false;
        }

        bool IsOverlappingForChunk(Vector2 candidate, List<Vector2> tempUsedPositions)
        {
            float minDistance = objectRadius * 2f;
            float minDistanceSqr = minDistance * minDistance;

            for (int i = 0; i < tempUsedPositions.Count; i++)
            {
                if ((tempUsedPositions[i] - candidate).sqrMagnitude < minDistanceSqr)
                {
                    return true;
                }
            }

            Collider2D hit = Physics2D.OverlapCircle(candidate, objectRadius, blockLayerMask);
            return hit != null;
        }

        void ApplyAsciiText(GameObject target, string ascii)
        {
            TMP_Text tmp = target.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = ascii;
                return;
            }

            TextMesh textMesh = target.GetComponentInChildren<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = ascii;
            }
        }

        void SetObjectColor(GameObject obj, float alpha)
        {
            TMP_Text tmp = obj.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
                return;
            }

            TextMesh textMesh = obj.GetComponentInChildren<TextMesh>();
            if (textMesh != null)
            {
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);
            }
        }

        string CreateRandomAsciiFromArray()
        {
            string baseAscii = asciiArts[Random.Range(0, asciiArts.Length)];
            if (string.IsNullOrEmpty(baseAscii))
            {
                return "#";
            }

            if (!useRandomSplit || baseAscii.Length == 1)
            {
                return baseAscii;
            }

            int safeMin = Mathf.Clamp(minSplitLength, 1, baseAscii.Length);
            int length = Random.Range(safeMin, baseAscii.Length + 1);
            int startIndex = Random.Range(0, baseAscii.Length - length + 1);
            return baseAscii.Substring(startIndex, length);
        }

        /// <summary>
        /// 戦闘中になった敵の処理
        /// </summary>
        public void OnBattle()
        {
            isBattleActive = true;
            foreach (var chunk in activeChunks.Values)
            {
                for (int i = 0; i < chunk.activeObjects.Count; i++)
                {
                    if (chunk.activeObjects[i] != null)
                    {
                        SetObjectColor(chunk.activeObjects[i], 0.3f);
                    }
                }
            }
        }

        /// <summary>
        /// 戦闘外の敵の処理
        /// </summary>
        public void OutBattle()
        {
            isBattleActive = false;
            foreach (var chunk in activeChunks.Values)
            {
                for (int i = 0; i < chunk.activeObjects.Count; i++)
                {
                    if (chunk.activeObjects[i] != null)
                    {
                        SetObjectColor(chunk.activeObjects[i], 1.0f);
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x, spawnArea.y, 0f));
        }

    }

}