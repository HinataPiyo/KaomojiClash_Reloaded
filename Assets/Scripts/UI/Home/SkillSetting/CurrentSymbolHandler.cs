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
        [SerializeField] SkillNameListHandler skillNameListHandler;
        [SerializeField] SkillSlotsController skillSlotsCTRL;

        public SymbolMouthData CurrentSymbolData { get; private set; }

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
            CurrentSymbolData = symbolData as SymbolMouthData;
            currentSymbolText.text = symbolData.Symbol.ToString();
            skillSlotsCTRL.SetSymbolData(symbolData);

            Debug.Log("Current symbol changed to: " + symbolData.Symbol.ToString());
        }

        public void AddOrUpdateSkillData(int slotIndex, SkillData addSkill)
        {
            // 指定されたスロットのスキルデータを更新(既にスキルが入っている場合は上書きされる)
            CurrentSymbolData.AddSkillData(slotIndex, addSkill);
        }
    }
}