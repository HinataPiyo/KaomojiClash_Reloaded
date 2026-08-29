using UnityEngine;

[CreateAssetMenu(fileName = "SymbolEyeData", menuName = "KaomojiClash_Reloaded/SymbolData/SymbolEyeData")]
public class SymbolEyeData : SymbolData
{
    [Header("EyeTypeのステータス")]
    // ステータスはレベルアップ時の伸びしろを示す。
    [SerializeField] CriticalStats criticalStats;

    public CriticalStats CriticalStats => criticalStats;
}

[System.Serializable]
public class CriticalStats
{
    public const float Max_CriticalDamage = 2.0f;
    public const float Max_CriticalRate = 1.0f;

    [Range(1, Max_CriticalDamage)] public float CriticalDamage;
    [Range(0, Max_CriticalRate)] public float CriticalRate;
}