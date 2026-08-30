namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SymbolIconButton : MonoBehaviour
    {
        SymbolData symbolData;
        [SerializeField] Button button;
        [SerializeField] TextMeshProUGUI symbolText;

        public void Initialize(SymbolData symbolData)
        {
            this.symbolData = symbolData;
            symbolText.text = symbolData.Symbol.ToString();
            button.onClick.AddListener(() => CurrentKaomojiHandler.OnCurrentKaomojiUpdated?.Invoke(symbolData));
        }
    }
}