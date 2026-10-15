namespace UI
{
    using TMPro;
    using UnityEngine;
    
    /// <summary>
    /// 現在選択されているシンボルを表示し、変更を通知するハンドラー
    /// </summary>
    public class CurrentSymbolHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI currentSymbolText;
        [SerializeField] SymbolListHandler symbolListHandler;
        [SerializeField] SkillSlotsController skillSlotsCTRL;

        void Awake()
        {
            symbolListHandler.OnSelectedSymbolChanged += OnCurrentSymbolChanged;
            currentSymbolText.text = "";
        }

        void OnDestroy()
        {
            symbolListHandler.OnSelectedSymbolChanged -= OnCurrentSymbolChanged;
        }

        /// <summary>
        /// 現在選択されている記号が変更されたときに呼び出されます。
        /// </summary>
        /// <param name="symbolData">変更された記号のデータ</param>
        void OnCurrentSymbolChanged(SymbolData symbolData)
        {
            // 現在の記号が変更されたときの処理をここに記述
            currentSymbolText.text = symbolData.Symbol.ToString();

            SymbolMouthData mouthData = symbolData as SymbolMouthData;
            if (mouthData != null) skillSlotsCTRL.SetSymbolData(mouthData);

            Debug.Log("Current symbol changed to: " + symbolData.Symbol.ToString());
        }
    }
}