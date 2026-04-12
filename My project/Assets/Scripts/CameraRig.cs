using UnityEngine;
using Unity.Cinemachine;

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

    private float _targetFOV;
    private float _targetDistance;

    private CinemachineThirdPersonFollow _thirdPersonFollow;

    private void Awake()
    {
        _thirdPersonFollow = thirdPersonCam.GetComponent<CinemachineThirdPersonFollow>();

        thirdPersonCam.Lens.FieldOfView = _baseFOV;
        if (_thirdPersonFollow != null)
        {
            _thirdPersonFollow.CameraDistance = _baseDistance;
            _thirdPersonFollow.ShoulderOffset = new Vector3(0, 0.6f, 0);
        }
    }

    public void UpdateCameraSettings(float currentRadius)
    {
        _targetFOV = Mathf.Clamp(_baseFOV + (currentRadius * _fovGrowthMultiplier), _baseFOV, _maxFOV);
        _targetDistance = _baseDistance + (currentRadius * _distanceMultiplier);

        if (_thirdPersonFollow != null)
        {
            _thirdPersonFollow.CameraDistance = _targetDistance;
        }
        thirdPersonCam.Lens.FieldOfView = _targetFOV;

        Debug.Log($"Camera Settings Updated - FOV: {_targetFOV}, Distance: {_targetDistance}");
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