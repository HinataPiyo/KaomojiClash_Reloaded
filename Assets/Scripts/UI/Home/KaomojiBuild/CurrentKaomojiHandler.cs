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
            currentKaomoji.OnStatusUpdated += OnStatusChanged;
            if (currentKaomoji.TryGetCurrentUpdate(out CurrentKaomojiUpdate update))
                OnStatusChanged(update);
        }

        protected virtual void OnDisable()
        {
            if (currentKaomoji != null)
            {
                currentKaomoji.OnCurrentKaomojiUpdated -= HandleCurrentKaomojiUpdated;
                currentKaomoji.OnStatusUpdated -= OnStatusChanged;
            }
        }

        void HandleCurrentKaomojiUpdated(CurrentKaomojiUpdate update)
        {
            OnCurrentKaomojiChanged(update);
        }

        protected abstract void OnCurrentKaomojiChanged(CurrentKaomojiUpdate update);

        protected virtual void OnStatusChanged(CurrentKaomojiUpdate update) { }
    }

    public class CurrentKaomojiHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI currentKaomojiText;
        [SerializeField] KaomojiData playerData;

        [SerializeField] Player.PlayerStatsCalculator statsCalc;

        SymbolListHandler symbolListHandler;
        CoreStats observedCoreStats;
        readonly HashSet<CriticalStats> observedCriticalStats = new HashSet<CriticalStats>();
        CurrentKaomojiUpdate currentUpdate;
        bool hasCurrentUpdate;

        // [イベント化] 購読側のみが登録できる、型付きの顔文字更新通知。
        public event System.Action<CurrentKaomojiUpdate> OnCurrentKaomojiUpdated;
        public event System.Action<CurrentKaomojiUpdate> OnStatusUpdated;

        void Start()
        {
            symbolListHandler = SymbolListHandler.Instance;
            symbolListHandler.OnSelectedSymbolChanged += UpdateCurrentKaomoji;
            SubscribeToStatusChanges();
            UpdateStatusViews();
        }

        void OnDestroy()
        {
            if (symbolListHandler != null)
                symbolListHandler.OnSelectedSymbolChanged -= UpdateCurrentKaomoji;
            UnsubscribeFromStatusChanges();
        }

        /// <summary>
        /// 現在の顔文字を更新します。
        /// </summary>
        /// <param name="symbolData">更新する顔文字のデータ</param>
        void UpdateCurrentKaomoji(SymbolData symbolData)
        {
            if (PanelChangeButton.CurrentPanel == PanelName.KaomojiBuild)
            {
                playerData.SetSymbolDataByType(symbolData);
                SubscribeToStatusChanges();
            }
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

            currentUpdate = new CurrentKaomojiUpdate(playerData, statusData);
            hasCurrentUpdate = true;
            OnCurrentKaomojiUpdated?.Invoke(currentUpdate);
        }

        /// <summary>
        /// 現在の更新を取得します。
        /// </summary>
        /// <param name="update">取得する更新</param>
        /// <returns>更新があるか</returns>
        public bool TryGetCurrentUpdate(out CurrentKaomojiUpdate update)
        {
            update = currentUpdate;
            return hasCurrentUpdate;
        }

        /// <summary>
        /// 顔文字のステータス変化を購読。
        /// </summary>
        void SubscribeToStatusChanges()
        {
            UnsubscribeFromStatusChanges();
            observedCoreStats = KaomojiSetUp.TryGetCoreStats(playerData);
            if (observedCoreStats != null)
                observedCoreStats.Changed += HandleStatusChanged;

            foreach (CriticalStats stats in KaomojiSetUp.TryGetCriticalStats(playerData))
            {
                if (stats != null && observedCriticalStats.Add(stats))
                    stats.Changed += HandleStatusChanged;
            }
        }

        /// <summary>
        /// 顔文字のステータス変化の購読を解除
        /// </summary>
        void UnsubscribeFromStatusChanges()
        {
            if (observedCoreStats != null)
                observedCoreStats.Changed -= HandleStatusChanged;
            observedCoreStats = null;

            foreach (CriticalStats stats in observedCriticalStats)
                stats.Changed -= HandleStatusChanged;
            observedCriticalStats.Clear();
        }

        /// <summary>
        /// ステータス変化時の処理
        /// </summary>
        void HandleStatusChanged()
        {
            statsCalc.SetUp(playerData);
            currentUpdate = new CurrentKaomojiUpdate(playerData, statsCalc.GetStatusDisplayData());
            hasCurrentUpdate = true;
            OnStatusUpdated?.Invoke(currentUpdate);
        }
    }
}