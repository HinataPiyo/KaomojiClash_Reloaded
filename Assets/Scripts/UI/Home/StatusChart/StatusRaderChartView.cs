namespace UI
{
    using Player;
    using TMPro;
    using UnityEngine;

    public class StatusRaderChartView : MonoBehaviour, ISetPlayerStatus
    {
        [SerializeField] StatusRadarChart radarChart;
        [SerializeField] Transform textParent;
        TextMeshProUGUI[] statusLabels;

        void Awake()
        {
            statusLabels = textParent.GetComponentsInChildren<TextMeshProUGUI>();
        }

        public void SetPlayerStatus(PlayerStatsCalculator calc)
        {
            if (radarChart == null || calc == null)
            {
                Debug.LogWarning("RadarChart or PlayerStatsCalculator is not assigned.");
                return;
            }

            RadarStatus[] radarStatuses = new RadarStatus[System.Enum.GetValues(typeof(StatusType)).Length];
            for (int i = 0; i < radarStatuses.Length; i++)
            {
                StatusType statusType = (StatusType)i;
                float value = 0f;
                float maxValue = 0f;
                float minValue = 0f;

                switch (statusType)
                {
                    case StatusType.Speed:
                        (value, maxValue) = calc.GetSpeed(CoreStats.Max_Speed);
                        minValue = CoreStats.Min_Speed;
                        statusLabels[i].text = $"Speed:{value}";
                        break;
                    case StatusType.Power:
                        (value, maxValue) = calc.GetPower(CoreStats.Max_Power);
                        minValue = CoreStats.Min_Power;
                        statusLabels[i].text = $"Power:{value}";
                        break;
                    case StatusType.Stamina:
                        (value, maxValue) = calc.GetStamina(CoreStats.Max_Stamina);
                        minValue = CoreStats.Min_Stamina;
                        statusLabels[i].text = $"Stamina:{value}";
                        break;
                    case StatusType.Guard:
                        (value, maxValue) = calc.GetGuard(CoreStats.Max_Guard);
                        minValue = CoreStats.Min_Guard;
                        statusLabels[i].text = $"Guard:{value}";
                        break;
                    case StatusType.CriticalDamage:
                        (value, maxValue) = calc.GetCriticalDamage(CriticalStats.Max_CriticalDamage);
                        minValue = CriticalStats.Min_CriticalDamage;
                        statusLabels[i].text = $"Critical\nDamage:{value}";
                        break;
                    case StatusType.CriticalRate:
                        (value, maxValue) = calc.GetCriticalRate(CriticalStats.Max_CriticalRate);
                        minValue = CriticalStats.Min_CriticalRate;
                        statusLabels[i].text = $"Critical\nRate:{value}";
                        break;
                }

                Debug.Log($"StatusType: {statusType}, Value: {value}, MaxValue: {maxValue}");

                radarStatuses[i] = new RadarStatus
                {
                    type = statusType,
                    value = value,
                    maxValue = maxValue,
                    minValue = minValue
                };
            }

            radarChart.SetValues(radarStatuses);
        }
    }

}