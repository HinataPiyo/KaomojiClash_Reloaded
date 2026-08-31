using UnityEngine;
using Wave;

public class RemainingTimeHandler : MonoBehaviour
{
    public static readonly float TIME_LIMIT = 33f;      // 演出があるので3秒多めに設定

    public float RemainingTime { get; private set; } = TIME_LIMIT;

    IBattleState battleState;
    UI.IRemainingTimeUI remainingTimeUI;

    void Start()
    {
        battleState = ApiProvider.Get<IBattleState>();
        remainingTimeUI = ApiProvider.Get<UI.IRemainingTimeUI>();
    }

    void Update()
    {
        // バトルステートがWaveInProgressでない場合は、残り時間のUIを非表示にする
        if (battleState.CurrentBattleState != BattleState.WaveInProgress)
        {
            // UIが表示されている場合は非表示にする
            remainingTimeUI.ShowRemainingTimeUI(false);
            RemainingTime = TIME_LIMIT;
            return;
        }

        // UIが表示されていない場合は表示する
        if(!remainingTimeUI.IsShowing) remainingTimeUI.ShowRemainingTimeUI(true);

        if (RemainingTime > 0f)
        {
            RemainingTime -= Time.deltaTime;

            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;
                battleState.ChangeBattleState(BattleState.TimeUp);
            }

            remainingTimeUI.UpdateRemainingTime(RemainingTime);
        }
    }
}