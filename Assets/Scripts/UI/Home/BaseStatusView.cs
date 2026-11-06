namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class BaseStatusView : CurrentKaomojiEventListener
    {
        StatusParamaterText[] statusParamaterTexts;

        void Awake()
        {
            statusParamaterTexts = GetComponentsInChildren<StatusParamaterText>();
        }

        // [設計変更] イベント受信を表示処理の入口にし、転送用メソッドをなくす。
        protected override void OnCurrentKaomojiChanged(CurrentKaomojiUpdate update)
        {
            UpdateStatuses(update);
        }

        protected override void OnStatusChanged(CurrentKaomojiUpdate update)
        {
            UpdateStatuses(update);
        }

        void UpdateStatuses(CurrentKaomojiUpdate update)
        {
            IReadOnlyList<StatusDisplayData> statusData = update.Statuses;
            if (statusData == null)
            {
                Debug.LogWarning("Status display data is not assigned.");
                return;
            }

            if (statusParamaterTexts.Length != statusData.Count)
            {
                Debug.LogWarning("Status display data count does not match the status text count.");
            }

            int count = Mathf.Min(statusParamaterTexts.Length, statusData.Count);
            for (int i = 0; i < count; i++)
            {
                StatusType statusType = statusData[i].Type;
                string titleText = string.Empty;
                string valueText = string.Empty;

                switch (statusType)
                {
                    case StatusType.Speed:
                        titleText = "Speed:";
                        valueText = $"{statusData[i].BaseValue}";
                        break;
                    case StatusType.Power:
                        titleText = "Power:";
                        valueText = $"{statusData[i].BaseValue}";
                        break;
                    case StatusType.Stamina:
                        titleText = "Stamina:";
                        valueText = $"{statusData[i].BaseValue}";
                        break;
                    case StatusType.Guard:
                        titleText = "Guard:";
                        valueText = $"{statusData[i].BaseValue}";
                        break;
                    case StatusType.CriticalDamage:
                        titleText = "CRIT Damage:";
                        valueText = statusData[i].Value.ToString("P1");
                        break;
                    case StatusType.CriticalRate:
                        titleText = "CRIT Rate:";
                        valueText = statusData[i].Value.ToString("P1");
                        break;
                }

                statusParamaterTexts[i].SetText(titleText, valueText);
            }
        }
    }
}