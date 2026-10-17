using UnityEngine;

[CreateAssetMenu(fileName = "Symbol_EyeData", menuName = "KaomojiClash_Reloaded/SymbolData/SymbolEyeData")]
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
    public const float Max_CriticalDamage = 1.5f;
    public const float Min_CriticalDamage = 0.0f;
    public const float Max_CriticalRate = 0.5f;
    public const float Min_CriticalRate = 0.0f;

    [Range(Min_CriticalDamage, Max_CriticalDamage)] public float CriticalDamage = Min_CriticalDamage;
    [Range(Min_CriticalRate, Max_CriticalRate)] public float CriticalRate = Min_CriticalRate;
}