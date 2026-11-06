using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class CriticalUIHandler : MonoBehaviour
{
    // 選択中の目記号とCritical数値を表示する参照。
    [SerializeField] TextMeshProUGUI select_Symbol_Text;

    // 選択または活性ノードの説明・状態と、選択ノードのActivate操作を表示する参照。
    [Header("ノード関連")]
    [SerializeField] TextMeshProUGUI critical_Node_Parameter;
    [SerializeField] TextMeshProUGUI clicked_Node_Description;
    [SerializeField] Button activateNodeButton;

    // 入力元と進行管理先は画面コンポーネントから取得して購読する。
    SymbolListHandler symbolListHandler;
    NodeController nodeController;
    SymbolEyeData selectedEye;

    // 必須参照を検証し、起動時にActivate操作を無効化する。
    void Awake()
    {
        nodeController = GetComponent<NodeController>();
        activateNodeButton.interactable = false;
    }

    // 画面表示中だけイベントとButton入力を購読する。
    void OnEnable()
    {
        nodeController.SelectedNodeChanged += RefreshNodeDetails;
        nodeController.ActiveNodeChanged += HandleActiveNodeChanged;
        nodeController.NodeStateChanged += HandleNodeStateChanged;
        nodeController.NodeActivated += HandleNodeActivated;

        activateNodeButton.onClick.AddListener(HandleActivateButtonClicked);

        if(selectedEye != null)
            selectedEye.CriticalStats.Changed += RefreshCriticalParameter;

        RefreshCriticalParameter();
        TrySubscribeToSymbolSelection();

        RefreshNodeDetails(null);
    }

    // SingletonのAwake順に依存しないよう、Start時にも購読を試みる。
    void Start()
    {
        TrySubscribeToSymbolSelection();
    }

    // 非表示時に全イベント購読を解除し、再表示時に重複購読しない。
    void OnDisable()
    {
        symbolListHandler.OnSelectedSymbolChanged -= HandleSymbolSelected;
        symbolListHandler = null;

        nodeController.SelectedNodeChanged -= RefreshNodeDetails;
        nodeController.ActiveNodeChanged -= HandleActiveNodeChanged;
        nodeController.NodeStateChanged -= HandleNodeStateChanged;
        nodeController.NodeActivated -= HandleNodeActivated;

        activateNodeButton.onClick.RemoveListener(HandleActivateButtonClicked);

        if(selectedEye != null)
            selectedEye.CriticalStats.Changed -= RefreshCriticalParameter;
    }

    // SymbolListHandlerが利用可能になってから目記号選択イベントを一度だけ購読する。
    void TrySubscribeToSymbolSelection()
    {
        symbolListHandler = SymbolListHandler.Instance;
        symbolListHandler.OnSelectedSymbolChanged += HandleSymbolSelected;
    }

    // 目記号選択時に数値と、その記号専用のノード進行状態を反映する。
    void HandleSymbolSelected(SymbolData data)
    {
        if (!(data is SymbolEyeData eye))
            return;

        if (selectedEye != null)
            selectedEye.CriticalStats.Changed -= RefreshCriticalParameter;
        selectedEye = eye;
        selectedEye.CriticalStats.Changed += RefreshCriticalParameter;
        select_Symbol_Text.text = eye.Symbol.ToString();
        RefreshCriticalParameter();
        nodeController.SetSymbol(eye);
    }

    // Activate操作をControllerへ渡し、成功・失敗どちらでもボタン状態を再評価する。
    void HandleActivateButtonClicked()
    {
        nodeController.TryActivateSelectedNode();
        RefreshNodeDetails(null);
    }

    // ノードの選択・状態変更を受けて説明後ろの状態ラベルと操作可否を更新する。
    void RefreshNodeDetails(NodeBase _)
    {
        NodeBase selectedNode = nodeController.SelectedNode;
        NodeBase displayedNode = selectedNode != null ? selectedNode : nodeController.ActiveNode;
        if (displayedNode == null)
        {
            clicked_Node_Description.text = "";
        }
        else
        {
            string description = string.IsNullOrWhiteSpace(displayedNode.Description)
                ? displayedNode.NodeName
                : displayedNode.Description;
            string state = displayedNode.IsActive
                ? "Active"
                : displayedNode.IsUnlocked ? "Unlocked" : "Locked";
            clicked_Node_Description.text = $"{description} ({state})";
        }

        activateNodeButton.interactable =
            selectedNode != null && selectedNode.IsUnlocked && !selectedNode.IsActive;
    }

    // 活性切替後は説明中ノードの状態表示も更新する。
    void HandleActiveNodeChanged(NodeBase node)
    {
        RefreshNodeDetails(node);
    }

    // 選択ノードの解放・ロック・活性変更に応じて説明とActivate可否を再評価する。
    void HandleNodeStateChanged(NodeBase node)
    {
        RefreshNodeDetails(node);
    }

    // Activate効果適用後に、その記号の更新済みCritical値を画面へ反映する。
    void HandleNodeActivated(NodeBase node, SymbolEyeData eyeSymbol)
    {
        RefreshNodeDetails(node);
    }

    // Critical率とCriticalダメージを既存の表示書式で更新する。
    void RefreshCriticalParameter()
    {
        if(selectedEye == null) return;
        
        float rate = selectedEye.CriticalStats.CriticalRate;
        float damage = selectedEye.CriticalStats.CriticalDamage;
        critical_Node_Parameter.text = $"critical rate : {rate:P1}\ncritical damage : {damage:P1}";
    }
}
