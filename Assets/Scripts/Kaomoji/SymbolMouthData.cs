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
    public float Stamina;
    public float Power;
    public float Speed;
    public float Defense;
}