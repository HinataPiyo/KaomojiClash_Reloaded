using System;
using UnityEngine;
using UnityEngine.UI;

public class NodeBase : MonoBehaviour
{
    // Node IDはSOの記号別進行状態を同じ種類のノードへ結び付ける固定キー。
    [SerializeField] string nodeId;
    // 説明が空の場合に説明欄へ表示する名前。
    [SerializeField] string nodeName;
    // 選択中のノードに表示する説明文。
    [SerializeField, TextArea] string description;
    // 解放状態を反映し、ノード選択を通知するButton。
    [SerializeField] Button nodeButton;
    // 記号別状態SOにエントリーがない場合の初期値。
    [SerializeField] bool unlockedOnStart;
    [SerializeField] bool activeOnStart;

    // 実行時にControllerが管理する進行状態。
    bool isUnlocked;
    bool isActive;

    // 状態をSOへ対応付ける安定識別子。
    public string NodeId => nodeId;
    // 接続グラフの辺と対応するButton。
    public Button NodeButton => nodeButton;
    // UI表示用タイトル。
    public string NodeName => string.IsNullOrWhiteSpace(nodeName) ? gameObject.name : nodeName;
    // UI表示用の説明文。
    public string Description => description;
    // SO初回登録時に使用する解放初期値。
    public bool DefaultUnlocked => unlockedOnStart;
    // SO初回登録時に使用する活性初期値。
    public bool DefaultActive => activeOnStart && unlockedOnStart;
    // 現在ロックされているか。
    public bool IsLocked => !isUnlocked;
    // 現在解放されているか。
    public bool IsUnlocked => isUnlocked;
    // 現在アクティブか。
    public bool IsActive => isActive;

    // クリック時にControllerへ選択要求を通知する。
    public event Action<NodeBase> Clicked;
    // 解放・活性状態が変化したときにControllerとViewへ通知する。
    public event Action<NodeBase> StateChanged;

    // Button操作を購読し、現在の解放状態を見た目へ反映する。
    void OnEnable()
    {
        if (nodeButton == null)
        {
            Debug.LogError("NodeBase requires a Button reference.", this);
            return;
        }

        nodeButton.interactable = isUnlocked;
        nodeButton.onClick.AddListener(HandleButtonClicked);
    }

    // Buttonイベントの二重購読を防ぐ。
    void OnDisable()
    {
        if (nodeButton != null)
            nodeButton.onClick.RemoveListener(HandleButtonClicked);
    }

    // Controllerまたは状態復元からノード状態を一括適用する。
    internal void ApplyState(bool unlocked, bool active, bool notify)
    {
        active = active && unlocked;
        bool stateChanged = isUnlocked != unlocked || isActive != active;
        isUnlocked = unlocked;
        isActive = active;

        if (nodeButton != null)
            nodeButton.interactable = isUnlocked;

        if (notify && stateChanged)
            StateChanged?.Invoke(this);
    }

    // 解放状態を維持したまま活性状態だけを変更する。
    internal void SetActiveState(bool active)
    {
        ApplyState(isUnlocked, active, true);
    }

    // 活性状態を維持できる範囲で解放状態を変更する。
    internal void SetUnlockedState(bool unlocked)
    {
        ApplyState(unlocked, isActive, true);
    }

    // ノード固有のActivate効果を派生クラスに委譲する。
    public virtual void ApplyActivationEffect(SymbolEyeData eyeSymbol)
    {
    }

    // 解放済みButtonの入力をControllerへ渡す。
    void HandleButtonClicked()
    {
        if (isUnlocked)
            Clicked?.Invoke(this);
    }
}
