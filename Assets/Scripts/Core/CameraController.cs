using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public interface ICamera
{
    void AddTargetToGroup(Transform target, float weight = 0.95f, float radius = 0f);
    void RemoveTargetFromGroup(Transform target);
    void SetCameraTarget(Transform target);
    void SetCameraState(CameraState state);
}

public enum CameraState
{ PlayerMoving, Encount, Battle }

public class CameraController : MonoBehaviour, ICamera
{
    CinemachineCamera cam;
    [SerializeField] CinemachineTargetGroup targetGroup;
    static readonly float PlayerMovingOrthoSize = 7f;
    static readonly float EncountOrthoSize = 4f;
    static readonly float BattleOrthoSize = 5f;

    void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        ApiProvider.Register<ICamera>(this);
    }

    public void SetCameraTarget(Transform target)
    {
        cam.Target.TrackingTarget = target;
        AddTargetToGroup(target, 1f, 1f);
    }

    /// <summary>
    /// カメラのTargetGroupにターゲットを追加する
    /// </summary>
    public void AddTargetToGroup(Transform target, float weight = 0.95f, float radius = 0.5f)
    {
        targetGroup.AddMember(target, weight, radius);
    }

    public void RemoveTargetFromGroup(Transform target)
    {
        targetGroup.RemoveMember(target);
    }

    public void SetCameraState(CameraState state)
    {
        switch (state)
        {
            case CameraState.PlayerMoving:
                StartCoroutine(SmoothOrthographicSize(PlayerMovingOrthoSize, 0.2f));
                break;
            case CameraState.Encount:
                StartCoroutine(SmoothOrthographicSize(EncountOrthoSize, 0.2f));
                break;
            case CameraState.Battle:
                cam.Target.TrackingTarget = targetGroup.transform;
                StartCoroutine(SmoothOrthographicSize(BattleOrthoSize, 0.2f));
                break;
        }
    }

    /// <summary>
    /// カメラのOrthographicSizeをスムーズに変更する
    /// </summary>
    /// <param name="targetSize">目標のOrthographicSize</param>
    /// <param name="duration">変更にかける時間</param>
    IEnumerator SmoothOrthographicSize(float targetSize, float duration)
    {
        float startSize = cam.Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easeOutT = t * (2f - t); // 最初は早く、徐々に遅くなる（EaseOutQuad）
            cam.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, easeOutT);
            yield return null;
        }

        cam.Lens.OrthographicSize = targetSize;
    }
}