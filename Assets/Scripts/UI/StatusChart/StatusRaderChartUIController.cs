using Player;
using UnityEngine;

public class StatusRaderChartUIController : MonoBehaviour
{
    [SerializeField] StatusRadarChart radarChart;

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
                    break;
                case StatusType.Power:
                    (value, maxValue) = calc.GetPower(CoreStats.Max_Power);
                    break;
                case StatusType.Stamina:
                    (value, maxValue) = calc.GetStamina(CoreStats.Max_Stamina);
                    break;
                case StatusType.Guard:
                    (value, maxValue) = calc.GetGuard(CoreStats.Max_Guard);
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