using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CameraFollow : PlayableBehaviour
{
    public GameObject targetObject;
    public float shakeIntensity;
    public float followSpeed;
    public float yOffset;
    public float zOffset;

    private Camera _mainCamera;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        _mainCamera = Camera.main;
        Debug.Log("CameraFollow behaviour started.");
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var target = targetObject.transform;

        if (_mainCamera == null || target == null)
            return;

        // Smoothly follow the target
        Vector3 desiredPosition = target.position + Vector3.up * yOffset - target.forward * zOffset;

        // Apply camera shake
        if (shakeIntensity > 0)
        {
            Vector3 shakeOffset = Mathf.Sin(Time.time * 20) * shakeIntensity * Vector3.one;
            desiredPosition += shakeOffset;
        }

        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, desiredPosition, followSpeed * Time.deltaTime);
        _mainCamera.transform.LookAt(target);
    }
}