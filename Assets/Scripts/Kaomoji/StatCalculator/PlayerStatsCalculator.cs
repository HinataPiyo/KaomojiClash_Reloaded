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
            return GetBaseStamina() * (1 + coreStats.Stamina);
        }

        public (float, float) GetStamina(float maxCoreStamina)
        {
            return (coreStats.Stamina, maxCoreStamina);
        }

#endregion

#region Power

        public float GetBasePower()
        {
            return statsConfig.Default_Power;
        }

        public override float GetPower()
        {
            return GetBasePower() * (1 + coreStats.Power) * combo.ComboDamageRate;
        }

        public (float, float) GetPower(float maxCorePower)
        {
            return (coreStats.Power, maxCorePower);
        }

#endregion

#region Speed

        public float GetBaseSpeed()
        {
            return statsConfig.Default_Speed;
        }

        public override float GetSpeed()
        {
            return GetBaseSpeed() * (1 + coreStats.Speed);
        }

        public (float, float) GetSpeed(float maxCoreSpeed)
        {
            return (coreStats.Speed, maxCoreSpeed);
        }

#endregion

#region Guard

        public float GetBaseGuard()
        {
            return statsConfig.Default_Guard;
        }

        public override float GetGuard()
        {
            return GetBaseGuard() * (1 + coreStats.Guard);
        }

        public (float, float) GetGuard(float maxCoreGuard)
        {
            return (coreStats.Guard, maxCoreGuard);
        }

#endregion


#region Critical

        public override float GetCriticalDamage()
        {
            if(criticalStats == null || criticalStats.Length == 0) return CriticalStats.Min_CriticalDamage;
            
            if(criticalStats.Length == 1) return criticalStats[0].CriticalDamage;
            else if(criticalStats.Length  == 2) return criticalStats[0].CriticalDamage + criticalStats[1].CriticalDamage;
            return CriticalStats.Min_CriticalDamage;
        }

        public (float, float) GetCriticalDamage(float maxCoreCriticalDamage)
        {
            if(criticalStats == null || criticalStats.Length == 0) return (CriticalStats.Min_CriticalDamage, CriticalStats.Min_CriticalDamage);
            if(criticalStats.Length == 1) return (criticalStats[0].CriticalDamage, maxCoreCriticalDamage);
            else if(criticalStats.Length == 2) return (criticalStats[0].CriticalDamage + criticalStats[1].CriticalDamage, maxCoreCriticalDamage);
            return (CriticalStats.Min_CriticalDamage, maxCoreCriticalDamage);
        }

        public override float GetCriticalRate()
        {
            if(criticalStats == null || criticalStats.Length == 0) return CriticalStats.Min_CriticalRate;
            if(criticalStats.Length == 1) return criticalStats[0].CriticalRate;
            else if(criticalStats.Length == 2) return criticalStats[0].CriticalRate + criticalStats[1].CriticalRate;
            return CriticalStats.Min_CriticalRate;
        }

        public (float, float) GetCriticalRate(float maxCoreCriticalRate)
        {
            if(criticalStats == null || criticalStats.Length == 0) return (CriticalStats.Min_CriticalRate, CriticalStats.Min_CriticalRate);
            if(criticalStats.Length == 1) return (criticalStats[0].CriticalRate, maxCoreCriticalRate);
            else if(criticalStats.Length == 2) return (criticalStats[0].CriticalRate + criticalStats[1].CriticalRate, maxCoreCriticalRate);
            return (CriticalStats.Min_CriticalRate, maxCoreCriticalRate);
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
            stats.TryGetSkill(out IWallReflection wallReflection);
            return GetSpeed() * wallReflection?.GetReflectionPower() ?? 0f;
        }
    }
}