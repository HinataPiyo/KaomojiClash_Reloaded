using UnityEngine;
using UnityEngine.InputSystem;
using Wave;

[RequireComponent(typeof(Collider2D))]
public class ArenaObjectDragAndDrop : MonoBehaviour
{
    [SerializeField] float gridSize = 1f;

    IBattleState battleState;
    IStage stage;
    InputAction press;
    InputAction point;

    Collider2D myCollider;
    bool isPressed;
    bool isDragging;
    Vector2 offset;
    Vector3 originalPosition;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        battleState = ApiProvider.Get<IBattleState>();
        stage = ApiProvider.Get<IStage>();
        press = InputSystem.actions["Press"];
        point = InputSystem.actions["Point"];
    }

    void Update()
    {
        if (battleState.CurrentBattleState != BattleState.ArenaObjectSelecting)
        {
            if (isDragging || isPressed)
            {
                isDragging = false;
                isPressed = false;
            }
            return;
        }

        bool isPressedNow = press.IsInProgress();

        // 押した瞬間
        if (isPressedNow && !isPressed)
        {
            isPressed = true;
            Vector2 screenPos = point.ReadValue<Vector2>();
            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(screenPos);

            if (myCollider != null && myCollider.OverlapPoint(touchWorldPos))
            {
                isDragging = true;
                originalPosition = transform.localPosition;
                offset = (Vector2)transform.position - touchWorldPos;
            }
        }
        else if (!isPressedNow && isPressed) // 離した瞬間
        {
            isPressed = false;
            if (isDragging)
            {
                isDragging = false;

                // エリア外、または他のオブジェクトと重なっている場合は元の位置に戻す
                if (!CheckSettingArea() || CheckOverlap())
                {
                    transform.localPosition = originalPosition;
                }
            }
        }

        // ドラッグ中の処理
        if (isDragging)
        {
            Vector2 screenPos = point.ReadValue<Vector2>();
            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
            Vector2 targetWorldPos = touchWorldPos + offset;

            // 親のローカル座標に変換
            Vector2 targetLocalPos = transform.parent != null 
                ? (Vector2)transform.parent.InverseTransformPoint(targetWorldPos) 
                : targetWorldPos;

            // グリッド移動（スナップ）
            float snappedLocalX = Mathf.Round(targetLocalPos.x / gridSize) * gridSize;
            float snappedLocalY = Mathf.Round(targetLocalPos.y / gridSize) * gridSize;

            // ステージの範囲内に Clamp する
            if (stage != null && stage.GetCurrentWall() != null)
            {
                Vector2 stageSize = stage.GetCurrentWall().GetWallRange();
                snappedLocalX = Mathf.Clamp(snappedLocalX, -stageSize.x / 2f, stageSize.x / 2f);
                snappedLocalY = Mathf.Clamp(snappedLocalY, -stageSize.y / 2f, stageSize.y / 2f);
            }

            transform.localPosition = new Vector3(snappedLocalX, snappedLocalY, transform.localPosition.z);
        }
    }

    /// <summary>
    /// 他のオブジェクトと重なっているか判定する
    /// </summary>
    bool CheckOverlap()
    {
        if (myCollider == null) return false;

        // トリガーコライダーも含めて衝突判定をする
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        
        Collider2D[] results = new Collider2D[10];
        int count = myCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] == null) continue;
            
            if (results[i].gameObject != gameObject)
            {
                // 自分以外の ArenaObjectDragAndDrop がアタッチされているオブジェクトとの重なりを検知
                if (results[i].GetComponent<ArenaObjectDragAndDrop>() != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// エリア外判定を取得する
    /// </summary>
    bool CheckSettingArea()
    {
        Vector2 position = transform.localPosition;
        Vector2 stageSize = stage.GetCurrentWall().GetWallRange();      // ステージの範囲を取得

        if (position.x < -stageSize.x / 2f || position.x > stageSize.x / 2f ||
            position.y < -stageSize.y / 2f || position.y > stageSize.y / 2f)
        {
            return false;
        }

        return true;
    }
}