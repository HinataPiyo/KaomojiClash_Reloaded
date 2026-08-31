namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public interface IRemainingTimeUI
    {
        void UpdateRemainingTime(float remainingTime);
        void ShowRemainingTimeUI(bool show);
        public bool IsShowing { get; }
    }

    public class RemainingTimeUI : MonoBehaviour, IRemainingTimeUI
    {
        [SerializeField] TextMeshProUGUI remainingTimeText;
        [SerializeField] Slider remainingTimeSlider;

        void Awake()
        {
            ApiProvider.Register<IRemainingTimeUI>(this);
        }

        public void UpdateRemainingTime(float remainingTime)
        {
            remainingTimeText.text = Mathf.CeilToInt(remainingTime).ToString() + "s";
            remainingTimeSlider.value = remainingTime / RemainingTimeHandler.TIME_LIMIT;
        }

        public bool IsShowing { get; private set; } = false;

        public void ShowRemainingTimeUI(bool show)
        {
            gameObject.SetActive(show);
            IsShowing = show;
        }
    }
}