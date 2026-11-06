using UnityEngine;
using UnityEngine.Serialization;

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
    public const float Max_CriticalRate = 0.25f;
    public const float Min_CriticalRate = 0.0f;

    [SerializeField, FormerlySerializedAs("CriticalDamage"), Range(Min_CriticalDamage, Max_CriticalDamage)]
    float criticalDamage = Min_CriticalDamage;
    [SerializeField, FormerlySerializedAs("CriticalRate"), Range(Min_CriticalRate, Max_CriticalRate)]
    float criticalRate = Min_CriticalRate;

    public event System.Action Changed;

    public float CriticalDamage
    {
        get => criticalDamage;
        set => SetValue(ref criticalDamage, value);
    }

    public float CriticalRate
    {
        get => criticalRate;
        set => SetValue(ref criticalRate, value);
    }

    void SetValue(ref float field, float value)
    {
        if (field.Equals(value))
            return;

        field = value;
        Changed?.Invoke();
    }
}