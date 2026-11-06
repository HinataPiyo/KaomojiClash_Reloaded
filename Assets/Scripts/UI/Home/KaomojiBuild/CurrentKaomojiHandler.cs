namespace UI
{
    using TMPro;
    using UnityEngine;
    
    public class CurrentKaomojiHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI currentKaomojiText;
        [SerializeField] KaomojiData playerData;

        [SerializeField] Player.PlayerStatsCalculator statsCalc;
        [SerializeField] StatusRaderChartView statusRaderChartView;
        [SerializeField] BaseStatusView baseStatusView;
        [SerializeField] SkillDescriptionHandler skillDescriptionHandler;

        SymbolListHandler symbolListHandler;

        public static System.Action<SymbolData> OnCurrentKaomojiUpdated;

        void Start()
        {
            symbolListHandler = SymbolListHandler.Instance;
            OnCurrentKaomojiUpdated += UpdateCurrentKaomoji;
            symbolListHandler.OnSelectedSymbolChanged += UpdateCurrentKaomoji;
            UpdateStatusViews();
        }

        void OnDestroy()
        {
            OnCurrentKaomojiUpdated -= UpdateCurrentKaomoji;
            if (symbolListHandler != null)
                symbolListHandler.OnSelectedSymbolChanged -= UpdateCurrentKaomoji;
        }

        /// <summary>
        /// 現在の顔文字を更新します。
        /// </summary>
        /// <param name="symbolData">更新する顔文字のデータ</param>
        public void UpdateCurrentKaomoji(SymbolData symbolData)
        {
            playerData.SetSymbolDataByType(symbolData);
            UpdateStatusViews();

            AudioManager.Instance.PlaySE(SEAudioName.ButtonClick_00);
        }

        /// <summary>
        /// 現在の顔文字とステータスビューを更新します。
        /// </summary>
        void UpdateStatusViews()
        {
            currentKaomojiText.text = KaomojiSetUp.GetKaomojiCoupling(playerData);
            statsCalc.SetUp(playerData);
            statusRaderChartView.SetPlayerStatus(statsCalc);
            baseStatusView.SetPlayerStatus(statsCalc);

            // AIリファクタリング: Viewの通知Actionを外部から呼ばず、明示的な表示APIを使う。
            skillDescriptionHandler.Clear();
        }
    }
}