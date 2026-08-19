namespace Player
{


    using UnityEngine;

    public interface IPlayerSpawn
    {
        void SpawnPlayer();
        GameObject PlayerInstance { get; }
        bool IsPlayerAlive { get; }
    }

    public class PlayerSpawn : MonoBehaviour, IPlayerSpawn
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] KaomojiData playerKaomojiData;

        GameObject playerInstance;

        // PlayerObject自体を返す
        public GameObject PlayerInstance => playerInstance;

        // PlayerObjectが存在するかどうかを返す
        public bool IsPlayerAlive => playerInstance != null;

        // PlayerObjectを生成する
        public void SpawnPlayer()
        {
            if (playerInstance == null)
            {
                playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                playerInstance.GetComponentInChildren<IKaomojiSetUp>().SetUp(playerKaomojiData);
            }
            else
            {
                Debug.Log("既にプレイヤーが存在します。");
            }
        }

        void Awake()
        {
            // IPlayerSpawnをApiProviderに登録
            ApiProvider.Register<IPlayerSpawn>(this);
        }

        void Start()
        {
            // ゲーム開始時にプレイヤーを生成
            SpawnPlayer();
        }
    }
}