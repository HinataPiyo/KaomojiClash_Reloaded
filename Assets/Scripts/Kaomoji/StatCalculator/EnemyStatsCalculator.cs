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
            return statsConfig.Default_Stamina * coreStats.Stamina;
        }

        public override float GetSpeed()
        {
            return statsConfig.Default_Speed * coreStats.Speed;
        }

        public override float GetPower()
        {
            return statsConfig.Default_Power * coreStats.Power;
        }

        public override float GetGuard()
        {
            return statsConfig.Default_Guard * coreStats.Guard;
        }

        public override float GetCriticalDamage()
        {
            return statsConfig.Default_CriticalDamage * criticalStats.CriticalDamage;
        }
        
        public override float GetCriticalRate()
        {
            return statsConfig.Default_CriticalRate * criticalStats.CriticalRate;
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