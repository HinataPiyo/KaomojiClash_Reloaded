namespace Player
{
    using UnityEngine;
    
    public class PlayerStatsCalculator : StatsCalculator
    {
        [SerializeField] PlayerCoreStatsConfig statsConfig;

        public override float GetStamina()
        {
            return statsConfig.Default_Stamina * coreStats.Stamina;
        }

        public override float GetPower()
        {
            return statsConfig.Default_Power * coreStats.Power;
        }

        public override float GetSpeed()
        {
            return statsConfig.Default_Speed * coreStats.Speed;
        }

        public override float GetDefense()
        {
            return statsConfig.Default_Defense * coreStats.Defense;
        }

        public float GetWallReflectionPower()
        {
            stats.TryGetSkill(out IWallReflection wallReflection, out int level);
            return GetSpeed() * wallReflection.GetReflectionPower(level);
        }
    }
}