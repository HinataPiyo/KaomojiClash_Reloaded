using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public interface ICamera
{
    void AddTargetToGroup(Transform target, float weight = 0.25f, float radius = 0.5f);
    void RemoveTargetFromGroup(Transform target);
    void SetCameraTarget(Transform target);
    void SetCameraState(CameraState state);
}

public enum CameraState
{ PlayerMoving, Encount, Battle }

public class CameraController : MonoBehaviour, ICamera
{
    [SerializeField] CinemachineCamera cam_TargetGroup;
    [SerializeField] CinemachineCamera cam_PlayerMoving;
    [SerializeField] CinemachineTargetGroup targetGroup;
    static readonly float PlayerMovingOrthoSize = 7f;
    static readonly float EncountOrthoSize = 4f;
    static readonly float BattleOrthoSize = 5f;

    const int ActivePriority = 10;
    const int InactivePriority = 0;

    Coroutine smoothCoroutine;

    enum CameraType
    { TargetGroup, PlayerMoving }

    void Awake()
    {
        ApiProvider.Register<ICamera>(this);
    }

    public void SetCameraTarget(Transform target)
    {
        cam_PlayerMoving.Target.TrackingTarget = target;
        AddTargetToGroup(target, 1f, 1f);
    }

    /// <summary>
    /// カメラのTargetGroupにターゲットを追加する
    /// </summary>
    public void AddTargetToGroup(Transform target, float weight = 0.25f, float radius = 0.5f)
    {
        targetGroup.AddMember(target, weight, radius);
    }

    public void RemoveTargetFromGroup(Transform target)
    {
        targetGroup.RemoveMember(target);
    }

    public void SetCameraState(CameraState state)
    {
        ResetSmoothCoroutine();

        switch (state)
        {
            case CameraState.PlayerMoving:
                smoothCoroutine = StartCoroutine(SmoothOrthographicSize(PlayerMovingOrthoSize, 0.2f, CameraType.PlayerMoving));
                break;
            case CameraState.Encount:
                smoothCoroutine = StartCoroutine(SmoothOrthographicSize(EncountOrthoSize, 0.2f, CameraType.PlayerMoving));
                break;
            case CameraState.Battle:
                cam_TargetGroup.Target.TrackingTarget = targetGroup.transform;
                smoothCoroutine = StartCoroutine(SmoothOrthographicSize(BattleOrthoSize, 0.2f, CameraType.TargetGroup));
                break;
        }
    }

    private void ResetSmoothCoroutine()
    {
        if (smoothCoroutine != null)
        {
            StopCoroutine(smoothCoroutine);
            smoothCoroutine = null;
        }
    }

    /// <summary>
    /// カメラのOrthographicSizeをスムーズに変更する
    /// </summary>
    /// <param name="targetSize">目標のOrthographicSize</param>
    /// <param name="duration">変更にかける時間</param>
    IEnumerator SmoothOrthographicSize(float targetSize, float duration, CameraType cameraType)
    {
        CinemachineCamera cam = (cameraType == CameraType.TargetGroup) ? cam_TargetGroup : cam_PlayerMoving;

        cam_TargetGroup.Priority = (cameraType == CameraType.TargetGroup) ? ActivePriority : InactivePriority;
        cam_PlayerMoving.Priority = (cameraType == CameraType.PlayerMoving) ? ActivePriority : InactivePriority;

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