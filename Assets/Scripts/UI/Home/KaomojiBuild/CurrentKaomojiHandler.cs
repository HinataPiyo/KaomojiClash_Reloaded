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

        public static System.Action<SymbolData> OnCurrentKaomojiUpdated;

        void Start()
        {
            OnCurrentKaomojiUpdated += UpdateCurrentKaomoji;
            UpdateStatusViews();
        }

        void OnDestroy()
        {
            OnCurrentKaomojiUpdated -= UpdateCurrentKaomoji;
        }

        /// <summary>
        /// 現在の顔文字を更新します。
        /// </summary>
        /// <param name="symbolData">更新する顔文字のデータ</param>
        public void UpdateCurrentKaomoji(SymbolData symbolData)
        {
            playerData.SetSymbolDataByType(symbolData);
            UpdateStatusViews();

            ApiProvider.Get<IAudioManager>().PlaySE(SEAudioName.ButtonClick_00);
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

            SkillNameListHandler.OnSkillNameListUpdated?.Invoke(playerData.GetAllSkillsWithLevels());
            SkillDescriptionHandler.OnSkillDescriptionCleared?.Invoke();
        }
    }
}