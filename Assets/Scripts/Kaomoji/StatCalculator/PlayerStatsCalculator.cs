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

        public override float GetStamina()
        {
            return statsConfig.Default_Stamina * coreStats.Stamina;
        }

        public (float, float) GetStamina(float maxCoreStamina)
        {
            return (statsConfig.Default_Stamina * coreStats.Stamina, statsConfig.Default_Stamina * maxCoreStamina);
        }

#endregion

#region Power

        public override float GetPower()
        {
            return statsConfig.Default_Power * coreStats.Power * combo.ComboDamageRate;
        }

        public (float, float) GetPower(float maxCorePower)
        {
            return (statsConfig.Default_Power * coreStats.Power, statsConfig.Default_Power * maxCorePower);
        }

#endregion

#region Speed

        public override float GetSpeed()
        {
            return statsConfig.Default_Speed * coreStats.Speed;
        }

        public (float, float) GetSpeed(float maxCoreSpeed)
        {
            if(coreStats == null)
            {
                Debug.LogWarning("CoreStats is null. Please check the KaomojiData setup.");
                return (0f, 0f);
            }
            return (statsConfig.Default_Speed * coreStats.Speed, statsConfig.Default_Speed * maxCoreSpeed);
        }

#endregion

#region Guard

        public override float GetGuard()
        {
            return statsConfig.Default_Guard * coreStats.Guard;
        }

        public (float, float) GetGuard(float maxCoreGuard)
        {
            return (statsConfig.Default_Guard * coreStats.Guard, statsConfig.Default_Guard * maxCoreGuard);
        }

#endregion


#region Critical
        public override float GetCriticalDamage()
        {
            if(criticalStats == null) return statsConfig.Default_CriticalDamage;
            return statsConfig.Default_CriticalDamage * criticalStats.CriticalDamage;
        }

        public (float, float) GetCriticalDamage(float maxCoreCriticalDamage)
        {
            if(criticalStats == null) return (0, statsConfig.Default_CriticalDamage);
            return (statsConfig.Default_CriticalDamage * criticalStats.CriticalDamage, statsConfig.Default_CriticalDamage * maxCoreCriticalDamage);
        }

        public override float GetCriticalRate()
        {
            if(criticalStats == null) return statsConfig.Default_CriticalRate;
            return statsConfig.Default_CriticalRate * criticalStats.CriticalRate;
        }

        public (float, float) GetCriticalRate(float maxCoreCriticalRate)
        {
            if(criticalStats == null) return (0, statsConfig.Default_CriticalRate);
            return (statsConfig.Default_CriticalRate * criticalStats.CriticalRate, statsConfig.Default_CriticalRate * maxCoreCriticalRate);
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
            return GetSpeed() * wallReflection.GetReflectionPower(level);
        }
    }
}