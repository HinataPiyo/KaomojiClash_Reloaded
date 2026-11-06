using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour
{
    // 記号別状態の保存先と、表示中のノード一覧・接続グラフ。
    [SerializeField] NodeProgressionDatabase progressionDatabase;
    [SerializeField] TreeConnectionGraphic connectionGraphic;
    [SerializeField] NodeBase[] nodes;

    // ノード所属判定と、接続辺の高速な出辺検索に使うランタイム索引。
    readonly HashSet<NodeBase> controlledNodes = new HashSet<NodeBase>();
    readonly Dictionary<Button, NodeBase> nodesByButton = new Dictionary<Button, NodeBase>();
    readonly Dictionary<NodeBase, HashSet<NodeBase>> outgoingNodes = new Dictionary<NodeBase, HashSet<NodeBase>>();
    SymbolData currentSymbol;

    // 選択は一時UI状態、ActiveNodeは記号別SOへ保存される進行状態。
    public NodeBase SelectedNode { get; private set; }
    public NodeBase ActiveNode { get; private set; }
    public SymbolData CurrentSymbol => currentSymbol;

    // UIはイベントを購読して選択・活性・状態・記号変更後に必要な範囲だけ更新する。
    public event Action<NodeBase> SelectedNodeChanged;
    public event Action<NodeBase> ActiveNodeChanged;
    public event Action<NodeBase> NodeStateChanged;
    public event Action<NodeBase, SymbolEyeData> NodeActivated;
    public event Action<SymbolData> SymbolChanged;

    // 初期参照を検証し、ノード識別索引と接続辺のキャッシュを構築する。
    void Awake()
    {
        if (progressionDatabase == null)
            throw new InvalidOperationException("NodeController requires a NodeProgressionDatabase.");
        if (connectionGraphic == null)
            throw new InvalidOperationException("NodeController requires a TreeConnectionGraphic.");

        if (nodes == null || nodes.Length == 0)
            nodes = GetComponentsInChildren<NodeBase>(true);
        if (nodes.Length == 0)
            throw new InvalidOperationException("NodeController requires at least one node.");

        HashSet<string> nodeIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (NodeBase node in nodes)
        {
            if (node == null)
                throw new InvalidOperationException("NodeController contains an unassigned node.");
            if (!controlledNodes.Add(node))
                throw new InvalidOperationException("NodeController contains the same node more than once.");
            if (string.IsNullOrWhiteSpace(node.NodeId) || !nodeIds.Add(node.NodeId))
                throw new InvalidOperationException("Every controlled node must have a unique, non-empty Node ID.");
            if (node.NodeButton == null || !nodesByButton.TryAdd(node.NodeButton, node))
                throw new InvalidOperationException("Every controlled node must have a unique, assigned Button.");

            outgoingNodes.Add(node, new HashSet<NodeBase>());
            node.ApplyState(node.DefaultUnlocked, node.DefaultActive, false);
            node.Clicked += HandleNodeClicked;
            node.StateChanged += HandleNodeStateChanged;
        }

        connectionGraphic.ConnectionsChanged += RebuildConnectionGraph;
        RebuildConnectionGraph();
    }

    // ノード・接続イベントを解除し、破棄後の参照を残さない。
    void OnDestroy()
    {
        if (connectionGraphic != null)
            connectionGraphic.ConnectionsChanged -= RebuildConnectionGraph;

        foreach (NodeBase node in nodes)
        {
            if (node == null)
                continue;

            node.Clicked -= HandleNodeClicked;
            node.StateChanged -= HandleNodeStateChanged;
        }
    }

    // 描画用TreeConnectionを唯一の接続定義として、出辺索引を作り直す。
    void RebuildConnectionGraph()
    {
        foreach (HashSet<NodeBase> destinations in outgoingNodes.Values)
            destinations.Clear();

        for (int i = 0; i < connectionGraphic.ConnectionCount; i++)
        {
            TreeConnection connection = connectionGraphic.GetConnection(i);
            if (connection == null || connection.Source == null || connection.Destination == null)
                throw new InvalidOperationException($"Tree connection {i} has a missing endpoint.");
            if (!nodesByButton.TryGetValue(connection.Source, out NodeBase source))
                throw new InvalidOperationException($"Tree connection {i} source is not controlled by this NodeController.");
            if (!nodesByButton.TryGetValue(connection.Destination, out NodeBase destination))
                throw new InvalidOperationException($"Tree connection {i} destination is not controlled by this NodeController.");

            // Visibleは描画設定であり、進行ルールはグラフに登録された全辺から独立して決まる。
            outgoingNodes[source].Add(destination);
        }
    }

    // 記号を切り替え、SOからその記号の解放・活性状態を表示ノードへ復元する。
    public void SetSymbol(SymbolData symbol)
    {
        if (symbol == null)
            throw new ArgumentNullException(nameof(symbol));
        if (currentSymbol == symbol)
            return;

        Dictionary<NodeBase, NodeProgressionState> states = new Dictionary<NodeBase, NodeProgressionState>();
        NodeBase nextActiveNode = null;
        foreach (NodeBase node in nodes)
        {
            NodeProgressionState state = progressionDatabase.GetOrCreateState(
                symbol,
                node.NodeId,
                node.DefaultUnlocked,
                node.DefaultActive);
            states.Add(node, state);

            // 複数の活性データがあっても表示上の単一アクティブ制約を維持する。
            if (nextActiveNode == null && state.IsUnlocked && state.IsActive)
                nextActiveNode = node;
        }

        currentSymbol = symbol;
        SelectedNode = null;
        ActiveNode = nextActiveNode;
        foreach (NodeBase node in nodes)
        {
            NodeProgressionState state = states[node];
            bool isActive = node == nextActiveNode;
            node.ApplyState(state.IsUnlocked, isActive, false);
            progressionDatabase.SetState(currentSymbol, node.NodeId, state.IsUnlocked, isActive);
        }

        SelectedNodeChanged?.Invoke(null);
        ActiveNodeChanged?.Invoke(ActiveNode);
        SymbolChanged?.Invoke(currentSymbol);
    }

    // 操作対象として選択できるのは、管理対象かつ現在解放済みのノードだけ。
    public bool TrySelectNode(NodeBase node)
    {
        if (currentSymbol == null || node == null || !controlledNodes.Contains(node) || !node.IsUnlocked)
            return false;
        if (SelectedNode == node)
            return true;

        SelectedNode = node;
        SelectedNodeChanged?.Invoke(SelectedNode);
        return true;
    }

    // 選択中ノードを活性化し、TreeConnectionGraphic上の全ての直後ノードを解放する。
    public bool TryActivateSelectedNode()
    {
        if (!(currentSymbol is SymbolEyeData eyeSymbol) ||
            SelectedNode == null ||
            !SelectedNode.IsUnlocked ||
            SelectedNode.IsActive)
            return false;
        if (!SetActiveNode(SelectedNode))
            return false;

        // 効果はActivate成功後に一度だけ適用し、UIへ反映済みの記号データを通知する。
        NodeBase activatedNode = SelectedNode;
        activatedNode.ApplyActivationEffect(eyeSymbol);
        foreach (NodeBase destination in outgoingNodes[SelectedNode])
            TryUnlockNode(destination);

        NodeActivated?.Invoke(activatedNode, eyeSymbol);
        return true;
    }

    // 既存呼び出し向けに、指定ノードを選択して通常の活性化フローへ渡す。
    public bool TrySetActiveNode(NodeBase node)
    {
        return TrySelectNode(node) && TryActivateSelectedNode();
    }

    // 記号が選択済みのときだけ管理対象ノードを解放し、SOへ即時反映する。
    public bool TryUnlockNode(NodeBase node)
    {
        if (currentSymbol == null || node == null || !controlledNodes.Contains(node))
            return false;

        node.SetUnlockedState(true);
        return true;
    }

    // ノードをロックし、該当する活性・選択状態も解除して一貫性を保つ。
    public bool TryLockNode(NodeBase node)
    {
        if (currentSymbol == null || node == null || !controlledNodes.Contains(node))
            return false;

        if (node == ActiveNode)
            ClearActiveNode();
        if (node == SelectedNode)
            ClearSelectedNode();

        node.SetUnlockedState(false);
        return true;
    }

    // 指定ノードを唯一の活性ノードにし、状態変化時だけ通知する。
    bool SetActiveNode(NodeBase node)
    {
        if (node == null || !node.IsUnlocked)
            return false;
        if (ActiveNode == node)
            return true;

        NodeBase previousActiveNode = ActiveNode;
        ActiveNode = node;
        if (previousActiveNode != null)
            previousActiveNode.SetActiveState(false);
        node.SetActiveState(true);
        ActiveNodeChanged?.Invoke(ActiveNode);
        return true;
    }

    // 現在の活性ノードを解除し、選択状態は変更せずに通知する。
    public void ClearActiveNode()
    {
        if (ActiveNode == null)
            return;

        NodeBase previousActiveNode = ActiveNode;
        ActiveNode = null;
        previousActiveNode.SetActiveState(false);
        ActiveNodeChanged?.Invoke(null);
    }

    // 現在の選択ノードを解除してUIへ通知する。
    void ClearSelectedNode()
    {
        if (SelectedNode == null)
            return;

        SelectedNode = null;
        SelectedNodeChanged?.Invoke(null);
    }

    // Node Buttonの入力は活性化ではなく選択として扱う。
    void HandleNodeClicked(NodeBase node)
    {
        TrySelectNode(node);
    }

    // 状態を記号別SOへ書き戻し、UIへ変更対象だけを通知する。
    void HandleNodeStateChanged(NodeBase node)
    {
        if (currentSymbol == null)
            return;

        progressionDatabase.SetState(currentSymbol, node.NodeId, node.IsUnlocked, node.IsActive);
        NodeStateChanged?.Invoke(node);
    }
}
