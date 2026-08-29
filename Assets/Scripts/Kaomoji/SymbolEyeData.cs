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
    public const float Min_CriticalDamage = 1.0f;
    public const float Max_CriticalRate = 1.0f;
    public const float Min_CriticalRate = 0.0f;

    [Range(Min_CriticalDamage, Max_CriticalDamage)] public float CriticalDamage;
    [Range(Min_CriticalRate, Max_CriticalRate)] public float CriticalRate;
}