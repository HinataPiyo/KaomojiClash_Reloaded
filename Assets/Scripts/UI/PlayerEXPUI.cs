namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public interface IPlayerEXPUI
    {
        void SetEXP(int currentEXP, int getEXP, int border, int level);
    }

    public class PlayerEXPUI : MonoBehaviour, IPlayerEXPUI
    {
        [SerializeField] Slider expSlider;
        [SerializeField] TextMeshProUGUI expText;

        [Header("Smooth Transition Settings")]
        [SerializeField] float expSpeed = 10f;      // expSliderの追従速度

        float targetEXPRatio = 0f;
        int currentLevel = 1;
        bool isInitialized = false;

        void Awake()
        {
            ApiProvider.Register<IPlayerEXPUI>(this);
        }

        void Update()
        {
            if (!isInitialized) return;

            // expSliderを滑らかに追従させる
            expSlider.value = Mathf.Lerp(expSlider.value, targetEXPRatio, Time.deltaTime * expSpeed);
        }

        public void SetEXP(int currentEXP, int getEXP, int border, int level)
        {
            // ExpHandlerに準じたMAX EXPの計算 (Level 1 = 100, Level 2 = 200...)
            int maxEXP = border;
            float newRatio = maxEXP > 0 ? (float)currentEXP / maxEXP : 0f;

            if (!isInitialized)
            {
                targetEXPRatio = newRatio;
                expSlider.value = newRatio;
                currentLevel = level;
                isInitialized = true;
            }
            else
            {
                // レベルアップした場合はスライダーを一度0にリセットして、新しい目標値に追従させる
                if (level > currentLevel)
                {
                    expSlider.value = 0f;
                }
                
                targetEXPRatio = newRatio;
                currentLevel = level;
            }

            expText.text = $"Lv {level}";
        }
    }
}