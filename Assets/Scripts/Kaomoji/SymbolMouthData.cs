using UnityEngine;

[CreateAssetMenu(fileName = "SymbolMouthData", menuName = "KaomojiClash_Reloaded/SymbolData/SymbolMouthData")]
public class SymbolMouthData : SymbolData
{
    [Header("MouthTypeのステータス")]
    // ステータスはレベルアップ時の伸びしろを示す。
    [SerializeField] CoreStats coreStats;

    public CoreStats CoreStats => coreStats;
}

[System.Serializable]
public class CoreStats
{
    [Range(1, 1.5f)] public float Stamina;
    [Range(1, 1.5f)] public float Power;
    [Range(1, 1.5f)] public float Speed;
    [Range(1, 1.5f)] public float Defense;
}