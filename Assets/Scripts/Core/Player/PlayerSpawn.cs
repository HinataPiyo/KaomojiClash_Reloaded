namespace Player
{
    using UnityEngine;
    using UI;

    public interface IPlayerSpawn
    {
        void SpawnPlayer();
        void MovementPlayerToEnemy(Vector2 enemyPosition);
        void MovementPlayerToWallCenter(Vector2 center);
        GameObject PlayerInstance { get; }
        bool IsPlayerAlive { get; }
        bool IsMoveToEnemy { get; }

        void SetInvincible(bool value);
    }

    public class PlayerSpawn : MonoBehaviour, IPlayerSpawn
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] KaomojiData playerKaomojiData;

        GameObject playerInstance;
        ICamera cam;
        IWaveStartWorldUI waveStartWorldUI;

        static readonly float encountDistance = 2f; // プレイヤーと敵のエンカウント距離
        Vector2 nextWaveEnemyPosition;
        Vector2 wallCenterPosition;
        public bool IsMoveToEnemy { get; private set; } = false;
        public bool IsMoveToWallCenter { get; private set; } = false;

        // PlayerObject自体を返す
        public GameObject PlayerInstance => playerInstance;

        // PlayerObjectが存在するかどうかを返す
        public bool IsPlayerAlive => playerInstance != null;


        void Awake()
        {
            // IPlayerSpawnをApiProviderに登録
            ApiProvider.Register<IPlayerSpawn>(this);
        }

        void Start()
        {
            cam = ApiProvider.Get<ICamera>();
            waveStartWorldUI = ApiProvider.Get<IWaveStartWorldUI>();

            // ゲーム開始時にプレイヤーを生成
            SpawnPlayer();
        }

        void Update()
        {
            // プレイヤーが敵の位置に移動する処理
            if(IsMoveToEnemy)
            {
                playerInstance.transform.position = Vector2.MoveTowards(playerInstance.transform.position, nextWaveEnemyPosition, Time.deltaTime * 15f);
                if (Vector2.Distance(playerInstance.transform.position, nextWaveEnemyPosition) <= encountDistance)
                {
                    cam.SetCameraState(CameraState.Encount);
                    // Playerと対象の敵との間の位置を取得する
                    Vector2 contactPosition = (playerInstance.transform.position + (Vector3)nextWaveEnemyPosition) / 2f;
                    waveStartWorldUI.ShowContactObjectUI(contactPosition);
                    IsMoveToEnemy = false;
                }
            }

            if(IsMoveToWallCenter)
            {
                if(playerInstance == null) return;
                playerInstance.transform.position = Vector2.MoveTowards(playerInstance.transform.position, wallCenterPosition, Time.deltaTime * 15f);
                if (Vector2.Distance(playerInstance.transform.position, wallCenterPosition) <= 0.1f)
                {
                    IsMoveToWallCenter = false;
                }
            }
        }

        /// <summary>
        /// プレイヤーを敵の位置まで移動させる
        /// </summary>
        /// <param name="enemyPosition">敵の位置</param>
        public void MovementPlayerToEnemy(Vector2 enemyPosition)
        {
            nextWaveEnemyPosition = enemyPosition;
            IsMoveToEnemy = true;
        }

        public void MovementPlayerToWallCenter(Vector2 center)
        {
            wallCenterPosition = center;
            IsMoveToWallCenter = true;
            SetInvincible(true);
        }

        /// <summary>
        /// プレイヤーの無敵状態を切り替える
        /// </summary>
        public void SetInvincible(bool value)
        {
            if(playerInstance == null) return;
            var invincible = playerInstance.GetComponent<Base.IInvincible>();
            if (invincible != null)
            {
                invincible.ChangeInvincible(value);
            }
            else
            {
                Debug.LogError($"<color=red>IInvincible component is missing on {playerInstance.name} (playerInstance). If you renamed 'Sutamina' to 'Stamina', the script might be detached from the prefab! Please check Player.prefab in the inspector.</color>");
            }
        }

        // PlayerObjectを生成する
        public void SpawnPlayer()
        {
            if (playerInstance == null)
            {
                playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                playerInstance.GetComponentInChildren<IKaomojiSetUp>().SetUp(playerKaomojiData);
                cam?.SetCameraTarget(playerInstance.transform);
            }
            else
            {
                Debug.Log("既にプレイヤーが存在します。");
            }
        }
    }
}