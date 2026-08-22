namespace UI
{
    using TMPro;
    using UnityEngine;
    public class ApplyDamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        public void SetText(float damage) {
            text.text = damage.ToString("F0");
        }
    }

}