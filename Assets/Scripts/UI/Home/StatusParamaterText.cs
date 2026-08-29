namespace UI
{
    using TMPro;
    using UnityEngine;

    public class StatusParamaterText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI value;

        public void SetText(string titleText, string valueText)
        {
            if(titleText != string.Empty)
            {
                title.text = titleText;
            }

            if(valueText != string.Empty)
            {
                value.text = valueText;
            }
        }
    }

}