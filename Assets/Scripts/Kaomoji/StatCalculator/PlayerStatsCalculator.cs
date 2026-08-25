namespace Player
{
    using UnityEngine;
    
    public class PlayerStatsCalculator : StatsCalculator
    {
        public override float GetStamina()
        {
            return 0;
        }

        public override float GetPower()
        {
            return 0;
        }

        public override float GetSpeed()
        {
            return coreStats.Speed;
        }

        public override float GetDefense()
        {
            return 0;
        }

        public float GetWallReflectionPower()
        {
            stats.TryGetSkill(out IWallReflection wallReflection, out int level);
            return GetSpeed() * wallReflection.GetReflectionPower(level);
        }
    }
}