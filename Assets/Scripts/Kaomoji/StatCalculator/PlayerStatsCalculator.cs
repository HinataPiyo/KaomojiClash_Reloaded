namespace Player
{
    using UnityEngine;
    
    public class PlayerStatsCalculator : StatsCalculator
    {
        [SerializeField] PlayerCoreStatsConfig statsConfig;
        ICombo combo;

        void Start()
        {
            if(SceneChange.Instance.CurrentScene == SceneName.Battle)
            {
                combo = Player.Combo.Instance;
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
            return (coreStats == null ? 0 : coreStats.Stamina, maxCoreStamina);
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
            return (coreStats == null ? 0 : coreStats.Power, maxCorePower);
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
            return (coreStats == null ? 0 : coreStats.Speed, maxCoreSpeed);
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
            return (coreStats == null ? 0 : coreStats.Guard, maxCoreGuard);
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

        // [設計変更] 表示に必要な値をここで一括化し、各Viewでの計算機呼び出しと重複計算を避ける。
        public StatusDisplayData[] GetStatusDisplayData()
        {
            var statusData = new StatusDisplayData[System.Enum.GetValues(typeof(StatusType)).Length];
            float criticalDamage = GetCriticalDamage();
            float criticalRate = GetCriticalRate();

            statusData[(int)StatusType.Stamina] = new StatusDisplayData(
                StatusType.Stamina, GetBaseStamina(), coreStats == null ? 0f : coreStats.Stamina,
                0f, CoreStats.Max_Stamina);
            statusData[(int)StatusType.Power] = new StatusDisplayData(
                StatusType.Power, GetBasePower(), coreStats == null ? 0f : coreStats.Power,
                0f, CoreStats.Max_Power);
            statusData[(int)StatusType.Speed] = new StatusDisplayData(
                StatusType.Speed, GetBaseSpeed(), coreStats == null ? 0f : coreStats.Speed,
                0f, CoreStats.Max_Speed);
            statusData[(int)StatusType.Guard] = new StatusDisplayData(
                StatusType.Guard, GetBaseGuard(), coreStats == null ? 0f : coreStats.Guard,
                0f, CoreStats.Max_Guard);
            statusData[(int)StatusType.CriticalDamage] = new StatusDisplayData(
                StatusType.CriticalDamage, criticalDamage, criticalDamage,
                0f, CriticalStats.Max_CriticalDamage * 2);
            statusData[(int)StatusType.CriticalRate] = new StatusDisplayData(
                StatusType.CriticalRate, criticalRate, criticalRate,
                0f, CriticalStats.Max_CriticalRate * 2);

            return statusData;
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