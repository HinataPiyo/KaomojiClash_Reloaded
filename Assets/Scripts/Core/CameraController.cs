using Unity.Cinemachine;
using UnityEngine;

public interface ICamera
{
    void SetCameraTarget(Transform target);
}

public class CameraController : MonoBehaviour, ICamera
{
    CinemachineCamera cam;

    void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        ApiProvider.Register<ICamera>(this);
    }

    public void SetCameraTarget(Transform target)
    {
        cam.Target.TrackingTarget = target;
    }
}