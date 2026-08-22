namespace Enemy
{
    using UnityEngine;
    using Wave;

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
        IMoveDirectionArrow moveDirectionArrow;
        Player.IPlayerSpawn playerSpawn;
        IWave wave;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            moveDirectionArrow = GetComponentInChildren<IMoveDirectionArrow>();
        }

        void Start()
        {
            playerSpawn = ApiProvider.Get<Player.IPlayerSpawn>();
            wave = ApiProvider.Get<IWave>();
            moveDirectionArrow.SetVisible(false);
        }

        void Update()
        {
            if(!playerSpawn.IsPlayerAlive || wave.IsStopCharacter) return;

            elapsedTime += Time.deltaTime;

            switch (currentState)
            {
                case MovementState.Idle:
                    if(elapsedTime >= idleTime)
                    {
                        currentState = MovementState.Dragging;
                        elapsedTime = 0f;
                        moveDirectionArrow.SetVisible(true);
                    }
                    break;

                case MovementState.Dragging:
                    // ドラッグ中の処理
                    currentTargetPosition = playerSpawn.PlayerInstance.transform.position;      // プレイヤーの位置を取得
                    startPosition = transform.position;      // 敵の位置を取得
                    launchDirection = (currentTargetPosition - startPosition).normalized;       // 発射方向を計算
                    moveDirectionArrow.SetArrowDirection(launchDirection);
                    moveDirectionArrow.UpdateScale(Vector2.Distance(startPosition, currentTargetPosition));    // 矢印の長さを更新s
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
                        moveDirectionArrow.SetVisible(false);
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