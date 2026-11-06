namespace UI
{
    using TMPro;
    using UnityEngine;
    
    /// <summary>
    /// AIリファクタリング: Presenterから受け取った現在のシンボルを表示するView。
    /// </summary>
    public class CurrentSymbolHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI currentSymbolText;

        void Awake()
        {
            currentSymbolText.text = "";
        }

        /// <summary>
        /// Presenterからシンボル選択を受け取り、表示だけを更新する。
        /// </summary>
        public void SetCurrentSymbol(SymbolData symbolData)
        {
            currentSymbolText.text = symbolData == null ? string.Empty : symbolData.Symbol.ToString();
        }
    }
}