namespace UI
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class StatusRaderChartView : CurrentKaomojiEventListener
    {
        [SerializeField] StatusRadarChart radarChart;
        [SerializeField] Transform textParent;
        TextMeshProUGUI[] statusLabels;

        void Awake()
        {
            statusLabels = textParent.GetComponentsInChildren<TextMeshProUGUI>();
        }

        // [設計変更] イベント受信をチャート描画の入口にし、転送用メソッドをなくす。
        protected override void OnCurrentKaomojiChanged(CurrentKaomojiUpdate update)
        {
            IReadOnlyList<StatusDisplayData> statusData = update.Statuses;
            if (radarChart == null || statusData == null)
            {
                Debug.LogWarning("RadarChart or status display data is not assigned.");
                return;
            }

            if (statusLabels.Length != statusData.Count)
            {
                Debug.LogWarning("Status display data count does not match the radar label count.");
            }

            RadarStatus[] radarStatuses = new RadarStatus[statusData.Count];
            for (int i = 0; i < statusData.Count; i++)
            {
                StatusDisplayData status = statusData[i];
                string label;
                switch (status.Type)
                {
                    case StatusType.Speed:
                        label = $"Speed: {status.Value.ToString("P0")}";
                        break;
                    case StatusType.Power:
                        label = $"Power: {status.Value.ToString("P0")}";
                        break;
                    case StatusType.Stamina:
                        label = $"Stamina: {status.Value.ToString("P0")}";
                        break;
                    case StatusType.Guard:
                        label = $"Guard: {status.Value.ToString("P0")}";
                        break;
                    case StatusType.CriticalDamage:
                        label = $"Critical\nDamage: {status.Value.ToString("P0")}";
                        break;
                    case StatusType.CriticalRate:
                        label = $"Critical\nRate: {status.Value.ToString("P0")}";
                        break;
                    default:
                        Debug.LogWarning($"Unsupported status type: {status.Type}");
                        label = status.Type.ToString();
                        break;
                }

                if (i < statusLabels.Length)
                {
                    statusLabels[i].text = label;
                }

                radarStatuses[i] = new RadarStatus
                {
                    type = status.Type,
                    value = status.Value,
                    maxValue = status.MaxValue,
                    minValue = status.MinValue
                };
            }

            radarChart.SetValues(radarStatuses);
        }
    }

}