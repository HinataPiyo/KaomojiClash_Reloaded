using Player;
using TMPro;
using UnityEngine;

public class StatusRaderChartUIController : MonoBehaviour
{
    [SerializeField] StatusRadarChart radarChart;
    [SerializeField] Transform textParent;
    TextMeshProUGUI[] statusLabels;

    void Awake()
    {
        statusLabels = textParent.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void UpdateRadarChart(PlayerStatsCalculator calc)
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

            switch (statusType)
            {
                case StatusType.Speed:
                    (value, maxValue) = calc.GetSpeed(CoreStats.Max_Speed);
                    statusLabels[i].text = $"Speed:{value}";
                    break;
                case StatusType.Power:
                    (value, maxValue) = calc.GetPower(CoreStats.Max_Power);
                    statusLabels[i].text = $"Power:{value}";
                    break;
                case StatusType.Stamina:
                    (value, maxValue) = calc.GetStamina(CoreStats.Max_Stamina);
                    statusLabels[i].text = $"Stamina:{value}";
                    break;
                case StatusType.Guard:
                    (value, maxValue) = calc.GetGuard(CoreStats.Max_Guard);
                    statusLabels[i].text = $"Guard:{value}";
                    break;
                case StatusType.CriticalDamage:
                    (value, maxValue) = calc.GetCriticalDamage(CriticalStats.Max_CriticalDamage);
                    statusLabels[i].text = $"Critical\nDamage:{value}";
                    break;
                case StatusType.CriticalRate:
                    (value, maxValue) = calc.GetCriticalRate(CriticalStats.Max_CriticalRate);
                    statusLabels[i].text = $"Critical\nRate:{value}";
                    break;
            }

            Debug.Log($"StatusType: {statusType}, Value: {value}, MaxValue: {maxValue}");

            radarStatuses[i] = new RadarStatus
            {
                type = statusType,
                value = value,
                maxValue = maxValue
            };
        }

        radarChart.SetValues(radarStatuses);
    }
}