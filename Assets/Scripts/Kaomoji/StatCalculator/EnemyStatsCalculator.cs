namespace Enemy
{
    using UnityEngine;
    
    public class EnemyStatsCalculator : StatsCalculator
    {
        [SerializeField] EnemyCoreStatsConfig statsConfig;

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
            return statsConfig.IdleTime;
        }

        public float GetDraggingIdleTime()
        {
            return statsConfig.DraggingIdleTime;
        }

        public float GetBeforeLaunchIdleDuration()
        {
            return statsConfig.BeforeLaunchIdleDuration;
        }

        public override float LaunchForce()
        {
            return GetSpeed() * GetMaxDraggingDistance();
        }
    }
}