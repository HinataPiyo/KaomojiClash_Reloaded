namespace UI
{
    using UnityEngine.UI;
    using UnityEngine;
    using TMPro;

    public class ResultArenaObjectIcon : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI levelText;

        public void SetIcon(Sprite sprite, int level)
        {
            icon.sprite = sprite;
            levelText.text = $"Lv{level}";
        }
    }
}