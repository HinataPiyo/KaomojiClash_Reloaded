using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SymbolHandData", menuName = "KaomojiClash_Reloaded/SymbolData/SymbolHandData")]
public class SymbolHandData : SymbolData
{
    [Header("HandTypeのステータス")]
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
    [SerializeField, FormerlySerializedAs("Stamina"), Range(0, Max_Stamina)] float stamina;
    [SerializeField, FormerlySerializedAs("Power"), Range(0, Max_Power)] float power;
    [SerializeField, FormerlySerializedAs("Speed"), Range(0, Max_Speed)] float speed;
    [SerializeField, FormerlySerializedAs("Guard"), Range(0, Max_Guard)] float guard;

    public event System.Action Changed;

    public float Stamina { get => stamina; set => SetValue(ref stamina, value); }
    public float Power { get => power; set => SetValue(ref power, value); }
    public float Speed { get => speed; set => SetValue(ref speed, value); }
    public float Guard { get => guard; set => SetValue(ref guard, value); }

    void SetValue(ref float field, float value)
    {
        if (field.Equals(value))
            return;

        field = value;
        Changed?.Invoke();
    }
}