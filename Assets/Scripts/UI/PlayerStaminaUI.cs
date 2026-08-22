namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public interface IPlayerStaminaUI
    {
        void UpdateStaminaUI(float currentStamina, float maxStamina);
    }

    public class PlayerStaminaUI : MonoBehaviour, IPlayerStaminaUI
    {
        [SerializeField] Slider staminaSlider;
        [SerializeField] Slider takeDamageSlider;
        [SerializeField] TextMeshProUGUI staminaText;

        [Header("Smooth Transition Settings")]
        [SerializeField] float staminaSpeed = 15f;      // staminaSliderの追従速度
        [SerializeField] float damageDelay = 0.5f;     // takeDamageSliderが減少し始めるまでの遅延時間（秒）
        [SerializeField] float damageSpeed = 5f;        // takeDamageSliderの追従速度

        float targetStaminaRatio = 1f;
        float delayTimer = 0f;
        bool isInitialized = false;

        void Awake()
        {
            ApiProvider.Register<IPlayerStaminaUI>(this);
        }

        void Update()
        {
            if (!isInitialized) return;

            // staminaSliderはすぐに滑らかに追従
            staminaSlider.value = Mathf.Lerp(staminaSlider.value, targetStaminaRatio, Time.deltaTime * staminaSpeed);

            // 被ダメージ用Sliderの遅延＆追従処理
            if (delayTimer > 0f)
            {
                delayTimer -= Time.deltaTime;
            }
            else
            {
                takeDamageSlider.value = Mathf.Lerp(takeDamageSlider.value, targetStaminaRatio, Time.deltaTime * damageSpeed);
            }

            // staminaSliderより値が小さくならないように制限（回復時等の不整合防止）
            if (takeDamageSlider.value < staminaSlider.value)
            {
                takeDamageSlider.value = staminaSlider.value;
            }
        }

        public void UpdateStaminaUI(float currentStamina, float maxStamina)
        {
            float newRatio = maxStamina > 0 ? currentStamina / maxStamina : 0f;

            if (!isInitialized)
            {
                targetStaminaRatio = newRatio;
                staminaSlider.value = newRatio;
                takeDamageSlider.value = newRatio;
                isInitialized = true;
            }
            else
            {
                // スタミナ減少（ダメージなど）時のみ遅延を発生
                if (newRatio < targetStaminaRatio)
                {
                    delayTimer = damageDelay;
                }
                // 回復時は即時追従できるようにディレイをリセット
                else if (newRatio > targetStaminaRatio)
                {
                    delayTimer = 0f;
                }

                targetStaminaRatio = newRatio;
            }

            staminaText.text = $"{Mathf.FloorToInt(currentStamina)}/{Mathf.FloorToInt(maxStamina)}";
        }
    }
}