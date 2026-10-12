namespace Enemy
{
    using UnityEngine;

    public interface IRandomOperationTime
    {
        float RandomIdleTimeModifier { get; }
        float RandomDraggingIdleTimeModifier { get; }
        float RandomBeforeLaunchIdleDurationModifier { get; }
        void SetRandomOperationTimeModifiers();
    }
    
    public class EnemyStatsCalculator : StatsCalculator, IRandomOperationTime
    {
        [SerializeField] EnemyCoreStatsConfig statsConfig;

        public float RandomIdleTimeModifier { get; private set; } = 1f;
        public float RandomDraggingIdleTimeModifier { get; private set; } = 1f;
        public float RandomBeforeLaunchIdleDurationModifier { get; private set; } = 1f;

        public void SetRandomOperationTimeModifiers()
        {
            RandomIdleTimeModifier = statsConfig.GetRandomModifiedIdleTime();
            RandomDraggingIdleTimeModifier = statsConfig.GetRandomModifiedDraggingIdleTime();
            RandomBeforeLaunchIdleDurationModifier = statsConfig.GetRandomModifiedBeforeLaunchIdleDuration();
        }

        public override float GetStamina()
        {
            return statsConfig.Default_Stamina * (1 + coreStats.Stamina);
        }

        public override float GetSpeed()
        {
            return statsConfig.Default_Speed * (1 + coreStats.Speed);
        }

        public override float GetPower()
        {
            return statsConfig.Default_Power * (1 + coreStats.Power);
        }

        public override float GetGuard()
        {
            return statsConfig.Default_Guard * (1 + coreStats.Guard);
        }

        public override float GetCriticalDamage()
        {
            if(criticalStats == null || criticalStats.Length == 0) return CriticalStats.Min_CriticalDamage;
            if(criticalStats.Length == 1) return criticalStats[0].CriticalDamage;
            else if(criticalStats.Length == 2) return criticalStats[0].CriticalDamage + criticalStats[1].CriticalDamage;
            return CriticalStats.Min_CriticalDamage;
        }
        
        public override float GetCriticalRate()
        {
            if(criticalStats == null || criticalStats.Length == 0) return CriticalStats.Min_CriticalRate;
            if(criticalStats.Length == 1) return criticalStats[0].CriticalRate;
            else if(criticalStats.Length == 2) return criticalStats[0].CriticalRate + criticalStats[1].CriticalRate;
            return CriticalStats.Min_CriticalRate;
        }

        // -- 攻撃関連 --

        public override float ApplyDamageCalculation()
        {
            return GetPower();
        }

        // -- 移動関連 --

        public override float GetMaxDraggingDistance()
        {
            return statsConfig.MaxDraggingDistance;
        }

        public float GetIdleTime()
        {
            return statsConfig.IdleTime * RandomIdleTimeModifier;
        }

        public float GetDraggingIdleTime()
        {
            return statsConfig.DraggingIdleTime * RandomDraggingIdleTimeModifier;
        }

        public float GetBeforeLaunchIdleDuration()
        {
            return statsConfig.BeforeLaunchIdleDuration * RandomBeforeLaunchIdleDurationModifier;
        }

        public override float LaunchForce()
        {
            return GetSpeed() * GetMaxDraggingDistance();
        }
    }
}