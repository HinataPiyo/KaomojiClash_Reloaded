namespace UI
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    // [設計変更] 顔文字更新のイベント値をまとめ、通知APIの拡張に強い型付き契約にする。
    public readonly struct CurrentKaomojiUpdate
    {
        public KaomojiData Data { get; }
        public IReadOnlyList<StatusDisplayData> Statuses { get; }

        internal CurrentKaomojiUpdate(KaomojiData data, IReadOnlyList<StatusDisplayData> statuses)
        {
            Data = data;
            Statuses = statuses;
        }
    }

    // [設計変更] イベント購読と解除を一元化し、各受信側を更新処理だけに専念させる。
    public abstract class CurrentKaomojiEventListener : MonoBehaviour
    {
        [SerializeField] CurrentKaomojiHandler currentKaomoji;

        protected virtual void OnEnable()
        {
            if (currentKaomoji == null)
            {
                Debug.LogError("CurrentKaomojiHandler is not assigned.", this);
                return;
            }

            currentKaomoji.OnCurrentKaomojiUpdated += HandleCurrentKaomojiUpdated;
        }

        protected virtual void OnDisable()
        {
            if (currentKaomoji != null)
                currentKaomoji.OnCurrentKaomojiUpdated -= HandleCurrentKaomojiUpdated;
        }

        void HandleCurrentKaomojiUpdated(CurrentKaomojiUpdate update)
        {
            OnCurrentKaomojiChanged(update);
        }

        protected abstract void OnCurrentKaomojiChanged(CurrentKaomojiUpdate update);
    }

    public class CurrentKaomojiHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI currentKaomojiText;
        [SerializeField] KaomojiData playerData;

        [SerializeField] Player.PlayerStatsCalculator statsCalc;

        SymbolListHandler symbolListHandler;

        // [イベント化] 購読側のみが登録できる、型付きの顔文字更新通知。
        public event System.Action<CurrentKaomojiUpdate> OnCurrentKaomojiUpdated;

        void Start()
        {
            symbolListHandler = SymbolListHandler.Instance;
            symbolListHandler.OnSelectedSymbolChanged += UpdateCurrentKaomoji;
            UpdateStatusViews();
        }

        void OnDestroy()
        {
            if (symbolListHandler != null)
                symbolListHandler.OnSelectedSymbolChanged -= UpdateCurrentKaomoji;
        }

        /// <summary>
        /// 現在の顔文字を更新します。
        /// </summary>
        /// <param name="symbolData">更新する顔文字のデータ</param>
        void UpdateCurrentKaomoji(SymbolData symbolData)
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
            StatusDisplayData[] statusData = statsCalc.GetStatusDisplayData();

            // [イベント化] 初期表示と選択後の更新を同じ通知経路で各Viewへ届ける。
            OnCurrentKaomojiUpdated?.Invoke(new CurrentKaomojiUpdate(playerData, statusData));
        }
    }
}