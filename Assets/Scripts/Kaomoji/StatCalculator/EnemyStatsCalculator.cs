namespace Enemy
{
    using UnityEngine;
    
    public class EnemyStatsCalculator : StatsCalculator
    {
        [SerializeField] EnemyCoreStatsConfig statsConfig;

        public override float GetStamina()
        {
            return statsConfig.Default_Stamina;
        }

        public override float GetSpeed()
        {
            return statsConfig.Default_Speed;
        }

        public override float GetPower()
        {
            return statsConfig.Default_Power;
        }

        public override float GetDefense()
        {
            return statsConfig.Default_Defense;
        }
    }
}