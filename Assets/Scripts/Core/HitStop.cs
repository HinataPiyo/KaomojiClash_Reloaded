using System.Collections;
using UnityEngine;

public interface IHitStop
{
    void HitStopEffect(float time = HitStop.DefaultHitStopTime);
    void PlayerDeathHitStopEffect(float time = HitStop.PlayerDeathHitStopTime);
}

public class HitStop : MonoBehaviour, IHitStop
{
    public const float DefaultHitStopTime = 0.05f;
    public const float PlayerDeathHitStopTime = 1f;
    Coroutine cor;

    void Awake()
    {
        ApiProvider.Register<IHitStop>(this);
    }

    public void HitStopEffect(float time = DefaultHitStopTime)
    {
        if(cor != null) StopCoroutine(cor);
        cor = StartCoroutine(HitStopRoutine(time));
    }

    /// <summary>
    /// プレイヤーが死亡したときのヒットストップ処理
    /// </summary>
    public void PlayerDeathHitStopEffect(float time = PlayerDeathHitStopTime)
    {
        if(cor != null) StopCoroutine(cor);
        cor = StartCoroutine(PlayerDeathHitStopRoutine(time));
    }

    IEnumerator HitStopRoutine(float time)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
        cor = null;
    }

    IEnumerator PlayerDeathHitStopRoutine(float time)
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
        cor = null;
    }


}