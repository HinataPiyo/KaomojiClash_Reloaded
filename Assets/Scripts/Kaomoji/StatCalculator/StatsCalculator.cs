using UnityEngine;

public interface IStatsCalculator
{
    void SetUp(IKaomojiStats kaomojiStats);
}


/// <summary>
/// Player,Enemyのステータスをまとめて計算し参照するクラス
/// </summary>
public abstract class StatsCalculator : MonoBehaviour, IStatsCalculator
{
    protected IKaomojiStats stats;
    protected CoreStats coreStats;

    public void SetUp(IKaomojiStats kaomojiStats)
    {
        stats = kaomojiStats;
        coreStats = stats.TryGetCoreStats();

        if(coreStats == null)
        {
            Debug.LogWarning("CoreStats is null. Please check the KaomojiData setup.");
        }
    }

    public abstract float GetStamina();
    public abstract float GetSpeed();
    public abstract float GetPower();
    public abstract float GetGuard();

    public abstract float ApplyDamageCalculation();

    public abstract float GetMaxDraggingDistance();
    public abstract float LaunchForce();
}