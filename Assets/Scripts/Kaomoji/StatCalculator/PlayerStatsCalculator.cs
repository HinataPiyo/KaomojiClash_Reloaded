namespace Player
{
    using UnityEngine;
    
    public class PlayerStatsCalculator : StatsCalculator
    {
        [SerializeField] PlayerCoreStatsConfig statsConfig;
        ICombo combo;

        void Start()
        {
            if(ApiProvider.Get<ISceneChange>().CurrentScene == SceneName.Battle)
            {
                combo = ApiProvider.Get<ICombo>();
            }
        }

// -- ステータス関連 --

#region Stamina

        public float GetBaseStamina()
        {
            return statsConfig.Default_Stamina;
        }

        public override float GetStamina()
        {
            return GetBaseStamina() * coreStats.Stamina;
        }

        public (float, float) GetStamina(float maxCoreStamina)
        {
            return (GetBaseStamina() * coreStats.Stamina, GetBaseStamina() * maxCoreStamina);
        }

#endregion

#region Power

        public float GetBasePower()
        {
            return statsConfig.Default_Power;
        }

        public override float GetPower()
        {
            return GetBasePower() * coreStats.Power * combo.ComboDamageRate;
        }

        public (float, float) GetPower(float maxCorePower)
        {
            return (GetBasePower() * coreStats.Power, GetBasePower() * maxCorePower);
        }

#endregion

#region Speed

        public float GetBaseSpeed()
        {
            return statsConfig.Default_Speed;
        }

        public override float GetSpeed()
        {
            return GetBaseSpeed() * coreStats.Speed;
        }

        public (float, float) GetSpeed(float maxCoreSpeed)
        {
            return (GetBaseSpeed() * coreStats.Speed, GetBaseSpeed() * maxCoreSpeed);
        }

#endregion

#region Guard

        public float GetBaseGuard()
        {
            return statsConfig.Default_Guard;
        }

        public override float GetGuard()
        {
            return GetBaseGuard() * coreStats.Guard;
        }

        public (float, float) GetGuard(float maxCoreGuard)
        {
            return (GetBaseGuard() * coreStats.Guard, GetBaseGuard() * maxCoreGuard);
        }

#endregion


#region Critical

        public override float GetCriticalDamage()
        {
            if(criticalStats == null) return statsConfig.Default_CriticalDamage;
            return statsConfig.Default_CriticalDamage + criticalStats.CriticalDamage;
        }

        public (float, float) GetCriticalDamage(float maxCoreCriticalDamage)
        {
            if(criticalStats == null) return (0, statsConfig.Default_CriticalDamage);
            return (statsConfig.Default_CriticalDamage + criticalStats.CriticalDamage, statsConfig.Default_CriticalDamage + maxCoreCriticalDamage);
        }

        public override float GetCriticalRate()
        {
            if(criticalStats == null) return statsConfig.Default_CriticalRate;
            return statsConfig.Default_CriticalRate + criticalStats.CriticalRate;
        }

        public (float, float) GetCriticalRate(float maxCoreCriticalRate)
        {
            if(criticalStats == null) return (0, statsConfig.Default_CriticalRate);
            return (statsConfig.Default_CriticalRate + criticalStats.CriticalRate, statsConfig.Default_CriticalRate + maxCoreCriticalRate);
        }

#endregion

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

        public override float LaunchForce()
        {
            return GetSpeed() * GetMaxDraggingDistance();
        }

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
            return GetSpeed() * wallReflection?.GetReflectionPower(level) ?? 0f;
        }
    }
}