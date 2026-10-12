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

    // 初期値は最小値
    // 強化は全てのパラメーターをランダムに増減させ決める
    [Range(0, Max_Stamina)] public float Stamina = 0f;
    [Range(0, Max_Power)] public float Power = 0f;
    [Range(0, Max_Speed)] public float Speed = 0f;
    [Range(0, Max_Guard)] public float Guard = 0f;
}