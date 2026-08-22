using System.Collections;
using UnityEngine;

public interface IHitStop
{
    void HitStopEffect(float time = HitStop.DefaultHitStopTime);
}

public class HitStop : MonoBehaviour, IHitStop
{
    public const float DefaultHitStopTime = 0.05f;
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

    IEnumerator HitStopRoutine(float time)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
        cor = null;
    }
}