namespace Player
{
    using UnityEngine;
    
    public class PlayerStatsCalculator : StatsCalculator
    {
        [SerializeField] PlayerCoreStatsConfig statsConfig;
        ICombo combo;

        void Start()
        {
            combo = ApiProvider.Get<ICombo>();
        }

        public override float GetStamina()
        {
            return statsConfig.Default_Stamina * coreStats.Stamina;
        }

        public override float GetPower()
        {
            return statsConfig.Default_Power * coreStats.Power * combo.ComboDamageRate;
        }

        public override float GetSpeed()
        {
            return statsConfig.Default_Speed * coreStats.Speed;
        }

        public override float GetDefense()
        {
            return statsConfig.Default_Defense * coreStats.Defense;
        }

        // -- 攻撃関連 --
        public override float ApplyDamageCalculation()
        {
            combo.AddCombo();
            return GetPower();
        }

        // -- 移動関連 --

        // 最大ドラッグ距離は、プレイヤーの移動範囲を制限するための値。
        public override float GetMaxDraggingDistance()
        {
            return statsConfig.MaxDraggingDistance;
        }

        public override float LaunchForce(){ return 0; }

        // プレイヤーの発射力は、ドラッグ距離と速度に基づいて計算される。
        public float LaunchForce(float length)
        {
            float distance = Mathf.Min(length, GetMaxDraggingDistance());
            return distance * GetSpeed();
        }

        // プレイヤーの壁反射力は、速度と壁反射スキルのレベルに基づいて計算される。
        public float GetWallReflectionPower()
        {
            stats.TryGetSkill(out IWallReflection wallReflection, out int level);
            return GetSpeed() * wallReflection.GetReflectionPower(level);
        }
    }
}