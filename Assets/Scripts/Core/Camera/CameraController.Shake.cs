using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public partial class CameraController
{
    CinemachineBasicMultiChannelPerlin noise;
    Coroutine cor_shake;

    private void InitializeShake()
    {
        noise = cam_TargetGroup.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        if (cor_shake != null) StopCoroutine(cor_shake);
        cor_shake = StartCoroutine(ShakeCameraRoutine(amplitude, frequency, duration));
    }

    IEnumerator ShakeCameraRoutine(float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;

        cor_shake = null;
    }
}