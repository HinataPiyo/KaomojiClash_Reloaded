namespace Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Wave;

    public class Movement : MonoBehaviour
    {
        [SerializeField] float default_Speed = 5f;
        [SerializeField] float max_DraggingDistance = 3f;

        IWave wave;

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
            press = InputSystem.actions["Press"];
            point = InputSystem.actions["Point"];
            wave = ApiProvider.Get<IWave>();
        }

        void Start()
        {
            wave = ApiProvider.Get<IWave>();
        }

        void Update()
        {
            if(wave.IsStopCharacter) return;

            // press.IsInProgress() はボタンが押されている間 true になる
            bool isPressedNow = press.IsInProgress();

            // 押した瞬間
            if (isPressedNow && !isPressed)
            {
                isPressed = true;
                startPosition = point.ReadValue<Vector2>();
                isDragging = true;
            }
            else if (!isPressedNow && isPressed)        // 離した瞬間
            {
                isPressed = false;
                isDragging = false;
                Impact();
            }

            // ドラッグ中の処理
            if(isDragging)
            {
                currentPosition = point.ReadValue<Vector2>();
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
            float distance = Mathf.Min(direction.magnitude, max_DraggingDistance);
            Vector2 force = -direction.normalized * distance * default_Speed;

            rb.AddForce(force, ForceMode2D.Impulse);
        }

    }
}