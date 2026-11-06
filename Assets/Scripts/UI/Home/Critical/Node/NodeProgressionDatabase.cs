using System;
using System.Collections.Generic;
using UnityEngine;

// 記号アセットごとのノード解放・活性状態をランタイムで保持する共有SO。
[CreateAssetMenu(fileName = "NodeProgressionDatabase", menuName = "KaomojiClash_Reloaded/Node Progression Database")]
public sealed class NodeProgressionDatabase : ScriptableObject
{
    // SymbolData参照とNode IDの組み合わせをキーにする状態一覧。
    [SerializeField] List<SymbolProgress> symbolProgress = new List<SymbolProgress>();

    // 既存状態を返し、初回利用時だけNodeBaseの既定値でエントリーを作る。
    internal NodeProgressionState GetOrCreateState(
        SymbolData symbol,
        string nodeId,
        bool defaultUnlocked,
        bool defaultActive)
    {
        SymbolProgress symbolEntry = GetOrCreateSymbol(symbol);
        NodeProgress nodeEntry = FindNode(symbolEntry, nodeId);
        if (nodeEntry == null)
        {
            nodeEntry = new NodeProgress
            {
                nodeId = nodeId,
                isUnlocked = defaultUnlocked,
                isActive = defaultActive && defaultUnlocked
            };
            symbolEntry.nodes.Add(nodeEntry);
        }

        return new NodeProgressionState(nodeEntry.isUnlocked, nodeEntry.isActive);
    }

    // 状態変更を対応する記号・Node IDのエントリーへ反映する。
    internal void SetState(SymbolData symbol, string nodeId, bool unlocked, bool active)
    {
        SymbolProgress symbolEntry = FindSymbol(symbol);
        if (symbolEntry == null)
            throw new InvalidOperationException("Node progression has not been initialized for this symbol.");

        NodeProgress nodeEntry = FindNode(symbolEntry, nodeId);
        if (nodeEntry == null)
            throw new InvalidOperationException($"Node ID '{nodeId}' has not been initialized for this symbol.");

        nodeEntry.isUnlocked = unlocked;
        nodeEntry.isActive = unlocked && active;
    }

    // 記号の状態一覧を検索し、未登録なら新規作成する。
    SymbolProgress GetOrCreateSymbol(SymbolData symbol)
    {
        if (symbol == null)
            throw new ArgumentNullException(nameof(symbol));

        SymbolProgress entry = FindSymbol(symbol);
        if (entry != null)
            return entry;

        entry = new SymbolProgress { symbol = symbol };
        symbolProgress.Add(entry);
        return entry;
    }

    // 重複エントリーを検出しながら指定記号の状態を検索する。
    SymbolProgress FindSymbol(SymbolData symbol)
    {
        SymbolProgress result = null;
        foreach (SymbolProgress entry in symbolProgress)
        {
            if (entry.symbol != symbol)
                continue;
            if (result != null)
                throw new InvalidOperationException($"Duplicate node progression entries exist for symbol '{symbol.name}'.");

            result = entry;
        }

        return result;
    }

    // 記号内のノード状態をIDで検索し、重複データを例外として通知する。
    static NodeProgress FindNode(SymbolProgress symbolEntry, string nodeId)
    {
        NodeProgress result = null;
        foreach (NodeProgress entry in symbolEntry.nodes)
        {
            if (!string.Equals(entry.nodeId, nodeId, StringComparison.Ordinal))
                continue;
            if (result != null)
                throw new InvalidOperationException($"Duplicate node progression entries exist for node ID '{nodeId}'.");

            result = entry;
        }

        return result;
    }

    // 1記号とそのノード状態群をUnityのシリアライズ対象にする。
    [Serializable]
    sealed class SymbolProgress
    {
        // 状態を所有する記号アセット。
        public SymbolData symbol;
        // 対象記号に属する各ノードの進行状態。
        public List<NodeProgress> nodes = new List<NodeProgress>();
    }

    // 1ノードの記号別解放・活性状態をシリアライズする。
    [Serializable]
    sealed class NodeProgress
    {
        // NodeBase.NodeIdに対応する安定キー。
        public string nodeId;
        // 対応ノードが解放されているか。
        public bool isUnlocked;
        // 対応ノードが活性状態か。
        public bool isActive;
    }
}

// ControllerとSO間で状態を受け渡す不変値型。
public readonly struct NodeProgressionState
{
    // 現在の解放状態。
    public bool IsUnlocked { get; }
    // ロック状態なら常にfalseとなる活性状態。
    public bool IsActive { get; }

    // 不正な「ロック中かつ活性」状態を作らずに値を構築する。
    public NodeProgressionState(bool isUnlocked, bool isActive)
    {
        IsUnlocked = isUnlocked;
        IsActive = isUnlocked && isActive;
    }
}
