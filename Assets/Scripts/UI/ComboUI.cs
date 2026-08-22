namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public interface IComboUI
    {
        void SetCombo(int combo, float speedUpRate, float powerUpRate);
    }

    public class ComboUI : MonoBehaviour, IComboUI
    {
        [SerializeField] Image comboImage;      // Filledを使用
        [SerializeField] TextMeshProUGUI comboText;
        [SerializeField] TextMeshProUGUI speedUpText;
        [SerializeField] TextMeshProUGUI powerUpText;
        Animator anim;

        void Awake()
        {
            ApiProvider.Register<IComboUI>(this);
            anim = GetComponentInChildren<Animator>();
            gameObject.SetActive(false);
        }

        public void SetCombo(int combo, float speedUpRate, float powerUpRate)
        {
            if (combo <= 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            anim.SetTrigger("Add");
            comboText.text = $"x{combo}";
            comboImage.fillAmount = (float)combo / Player.Combo.ComboMaxCount;
            speedUpText.text = $"speed up: {speedUpRate * 100f:0}%";
            powerUpText.text = $"power up: {powerUpRate * 100f:0}%";
        }

    }
}