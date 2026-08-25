namespace Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Wave;

    public class Movement : MonoBehaviour
    {
        IWave wave;
        IMoveDirectionArrow moveDirectionArrow;
        UI.IPlayerHereArrow playerHereArrow;

        PlayerStatsCalculator statsCalc;

        Rigidbody2D rb;
        InputAction press;
        InputAction point;

        Vector2 startPosition;
        Vector2 currentPosition;
        bool isDragging;
        bool isPressed;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            statsCalc = GetComponent<PlayerStatsCalculator>();
            moveDirectionArrow = GetComponentInChildren<IMoveDirectionArrow>();
            press = InputSystem.actions["Press"];
            point = InputSystem.actions["Point"];
        }

        void Start()
        {
            wave = ApiProvider.Get<IWave>();
            moveDirectionArrow.SetVisible(false);
            playerHereArrow = ApiProvider.Get<UI.IPlayerHereArrow>();
        }

        void Update()
        {
            playerHereArrow.UpdatePlayerHereArrowPosition(transform.position);
            if(wave.IsStopCharacter) return;

            // press.IsInProgress() はボタンが押されている間 true になる
            bool isPressedNow = press.IsInProgress();

            // 押した瞬間
            if (isPressedNow && !isPressed)
            {
                isPressed = true;
                startPosition = point.ReadValue<Vector2>();
                isDragging = true;
                moveDirectionArrow.SetVisible(true);
            }
            else if (!isPressedNow && isPressed)        // 離した瞬間
            {
                isPressed = false;
                isDragging = false;
                moveDirectionArrow.SetVisible(false);
                Impact();
            }

            // ドラッグ中の処理
            if(isDragging)
            {
                currentPosition = point.ReadValue<Vector2>();

                // スクリーン（ピクセル）座標からワールド座標に変換して計算
                Vector2 startWorldPos = Camera.main.ScreenToWorldPoint(startPosition);
                Vector2 currentWorldPos = Camera.main.ScreenToWorldPoint(currentPosition);

                // 引っ張った方向と逆（飛んでいく方向）を矢印の向きとする
                Vector2 dragDirection = startWorldPos - currentWorldPos;
                float distance = dragDirection.magnitude;

                moveDirectionArrow.SetArrowDirection(dragDirection);
                moveDirectionArrow.UpdateScale(distance);
            }
        }

        /// <summary>
        /// プレイヤーに力を加える処理
        /// </summary>
        void Impact()
        {
            rb.linearVelocity = Vector2.zero;

            // スクリーン（ピクセル）座標からワールド座標に変換して計算
            Vector2 startWorldPos = Camera.main.ScreenToWorldPoint(startPosition);
            Vector2 currentWorldPos = Camera.main.ScreenToWorldPoint(currentPosition);

            Vector2 direction = currentWorldPos - startWorldPos;
            Vector2 force = -direction.normalized * statsCalc.LaunchForce(direction.magnitude);

            rb.AddForce(force, ForceMode2D.Impulse);
        }

    }
}