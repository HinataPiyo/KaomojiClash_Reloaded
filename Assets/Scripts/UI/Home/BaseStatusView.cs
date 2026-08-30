namespace UI
{
    using UnityEngine;
    
    public interface ISetPlayerStatus
    {
        void SetPlayerStatus(Player.PlayerStatsCalculator calc);
    }
    
    public class BaseStatusView : MonoBehaviour, ISetPlayerStatus
    {
        StatusParamaterText[] statusParamaterTexts;

        void Awake()
        {
            statusParamaterTexts = GetComponentsInChildren<StatusParamaterText>();
        }

        public void SetPlayerStatus(Player.PlayerStatsCalculator calc)
        {
            if (calc == null)
            {
                Debug.LogWarning("PlayerStatsCalculator is not assigned.");
                return;
            }

            for (int i = 0; i < statusParamaterTexts.Length; i++)
            {
                StatusType statusType = (StatusType)i;
                string titleText = string.Empty;
                string valueText = string.Empty;

                switch (statusType)
                {
                    case StatusType.Speed:
                        titleText = "Speed:";
                        valueText = $"{calc.GetBaseSpeed()}";
                        break;
                    case StatusType.Power:
                        titleText = "Power:";
                        valueText = $"{calc.GetBasePower()}";
                        break;
                    case StatusType.Stamina:
                        titleText = "Stamina:";
                        valueText = $"{calc.GetBaseStamina()}";
                        break;
                    case StatusType.Guard:
                        titleText = "Guard:";
                        valueText = $"{calc.GetBaseGuard()}";
                        break;
                    case StatusType.CriticalDamage:
                        float criticalDamage = calc.GetCriticalDamage(CriticalStats.Max_CriticalDamage).Item1;
                        titleText = "CRIT Damage:";
                        valueText = $"{criticalDamage}";
                        break;
                    case StatusType.CriticalRate:
                        float criticalRate = calc.GetCriticalRate(CriticalStats.Max_CriticalRate).Item1;
                        titleText = "CRIT Rate:";
                        valueText = $"{criticalRate}";
                        break;
                }

                statusParamaterTexts[i].SetText(titleText, valueText);
            }
        }
    }
}