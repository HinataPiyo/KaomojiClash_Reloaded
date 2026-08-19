namespace Enemy
{
    using UnityEngine;
    
    public class Movement : MonoBehaviour
    {
        enum MovementState
        {
            Idle,
            Dragging,
            BeforeLaunchIdle
        }

        [SerializeField] float default_Speed = 5f;
        [SerializeField] float idleTime = 1f;                        // 何もしない時間
        [SerializeField] float max_DraggingDistance = 3f;
        [SerializeField] float draggingIdleTime = 1f;                // ドラッグ中の待機時間
        [SerializeField] float beforeLaunchIdleDuration = 0.25f;     // 発射方向が確定したのちの待機時間

        Rigidbody2D rb;
        Vector2 startPosition;
        Vector2 currentTargetPosition;
        Vector2 launchDirection;

        float elapsedTime;
        MovementState currentState = MovementState.Idle;
        Player.IPlayerSpawn playerSpawn;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            playerSpawn = ApiProvider.Get<Player.IPlayerSpawn>();
        }

        void Update()
        {
            if(!playerSpawn.IsPlayerAlive) return;

            elapsedTime += Time.deltaTime;

            switch (currentState)
            {
                case MovementState.Idle:
                    if(elapsedTime >= idleTime)
                    {
                        currentState = MovementState.Dragging;
                        elapsedTime = 0f;
                    }
                    break;

                case MovementState.Dragging:
                    // ドラッグ中の処理
                    currentTargetPosition = playerSpawn.PlayerInstance.transform.position;      // プレイヤーの位置を取得
                    startPosition = transform.position;      // 敵の位置を取得
                    launchDirection = (currentTargetPosition - startPosition).normalized;       // 発射方向を計算
                    if (elapsedTime >= draggingIdleTime)
                    {
                        currentState = MovementState.BeforeLaunchIdle;
                        elapsedTime = 0f;
                    }
                    break;

                case MovementState.BeforeLaunchIdle:
                    // 発射方向が確定したのちの待機時間
                    if (elapsedTime >= beforeLaunchIdleDuration)
                    {
                        currentState = MovementState.Idle;
                        elapsedTime = 0f;
                        Launch();
                    }
                    break;
            }
        }

        void Launch()
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 force = launchDirection * default_Speed * max_DraggingDistance;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}