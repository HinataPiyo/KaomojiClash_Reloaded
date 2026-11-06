using UnityEngine;

public interface IStatsCalculator
{
    void SetUp(IKaomojiStats kaomojiStats);
}

// [設計変更] 計算結果をViewへ渡す共通データ。UI側が計算機の詳細に依存しないようにする。
public readonly struct StatusDisplayData
{
    public StatusType Type { get; }
    public float BaseValue { get; }
    public float Value { get; }
    public float MinValue { get; }
    public float MaxValue { get; }

    public StatusDisplayData(StatusType type, float baseValue, float value, float minValue, float maxValue)
    {
        Type = type;
        BaseValue = baseValue;
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
    }
}

/// <summary>
/// Player,Enemyのステータスをまとめて計算し参照するクラス
/// </summary>
public abstract class StatsCalculator : MonoBehaviour, IStatsCalculator
{
    protected IKaomojiStats stats;
    protected CoreStats coreStats;
    protected CriticalStats[] criticalStats;

    public void SetUp(IKaomojiStats kaomojiStats)
    {
        stats = kaomojiStats;
        coreStats = KaomojiSetUp.TryGetCoreStats(stats.Data);
        criticalStats = KaomojiSetUp.TryGetCriticalStats(stats.Data);

        if(coreStats == null)
        {
            Debug.LogWarning("CoreStats is null. Please check the KaomojiData setup.");
        }
    }

    public void SetUp(KaomojiData kaomojiData)
    {
        coreStats = KaomojiSetUp.TryGetCoreStats(kaomojiData);
        criticalStats = KaomojiSetUp.TryGetCriticalStats(kaomojiData);
    }

    public abstract float GetStamina();
    public abstract float GetSpeed();
    public abstract float GetPower();
    public abstract float GetGuard();

    public abstract float GetCriticalDamage();
    public abstract float GetCriticalRate();

    public abstract float ApplyDamageCalculation();

    public abstract float GetMaxDraggingDistance();
    public abstract float LaunchForce();
}