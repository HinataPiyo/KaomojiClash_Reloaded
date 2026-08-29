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
    public const float Max_Stamina = 1.5f;
    public const float Max_Power = 1.5f;
    public const float Max_Speed = 1.5f;
    public const float Max_Guard = 1.5f;

    [Range(1, Max_Stamina)] public float Stamina;
    [Range(1, Max_Power)] public float Power;
    [Range(1, Max_Speed)] public float Speed;
    [Range(1, Max_Guard)] public float Guard;
}