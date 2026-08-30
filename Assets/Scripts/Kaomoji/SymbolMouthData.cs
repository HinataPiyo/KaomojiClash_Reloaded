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
    public const float Min_Stamina = 1f;
    public const float Max_Power = 1.5f;
    public const float Min_Power = 1f;
    public const float Max_Speed = 1.5f;
    public const float Min_Speed = 1f;
    public const float Max_Guard = 1.5f;
    public const float Min_Guard = 1f;

    [Range(Min_Stamina, Max_Stamina)] public float Stamina;
    [Range(Min_Power, Max_Power)] public float Power;
    [Range(Min_Speed, Max_Speed)] public float Speed;
    [Range(Min_Guard, Max_Guard)] public float Guard;
}