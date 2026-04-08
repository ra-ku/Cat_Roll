using UnityEngine;
using Unity.Cinemachine; // 최신 버전 네임스페이스

public class CameraRig : MonoBehaviour
{
    public enum CameraState
    {
        FirstPerson,
        ThirdPerson,
    }

    [Header("Virtual Cameras")]
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;

    [Header("FOV Settings")]
    [SerializeField] private float _baseFOV = 60f;
    [SerializeField] private float _fovGrowthMultiplier = 2f;
    [SerializeField] private float _maxFOV = 90f;

    [Header("Distance Settings (3rd Person)")]
    [SerializeField] private float _baseDistance = 1.5f;
    [SerializeField] private float _distanceMultiplier = 1.1f;

    [Header("Smoothing")]
    [SerializeField] private float _smoothSpeed = 5f;

    private float _targetFOV;
    private float _targetDistance;

    private CinemachineOrbitalFollow _thirdPersonOrbital;

    private void Awake()
    { 
        _thirdPersonOrbital = thirdPersonCam.GetComponent<CinemachineOrbitalFollow>();

        thirdPersonCam.Lens.FieldOfView = _baseFOV;
        _thirdPersonOrbital.TargetOffset.z = _baseDistance;
    }

    public void UpdateCameraSettings(float currentRadius)
    {
        _targetFOV = Mathf.Clamp(_baseFOV + (currentRadius * _fovGrowthMultiplier), _baseFOV, _maxFOV);
        _targetDistance = _baseDistance + -(currentRadius * _distanceMultiplier);

        Debug.Log($"Fov = {_targetFOV}");
        Debug.Log($"Distance = {_targetDistance}");

        _thirdPersonOrbital.TargetOffset.z = _targetDistance;
        thirdPersonCam.Lens.FieldOfView = _targetFOV;
    }

    public void SetState(CameraState state)
    {
        firstPersonCam.Priority = 10;
        thirdPersonCam.Priority = 10;

        switch (state)
        {
            case CameraState.FirstPerson:
                firstPersonCam.Priority = 20;
                break;
            case CameraState.ThirdPerson:
                thirdPersonCam.Priority = 20;
                break;
        }
    }
}