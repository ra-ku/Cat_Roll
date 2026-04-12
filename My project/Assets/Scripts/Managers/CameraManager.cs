using UnityEngine;

public class CameraManager : IManager, IUpdater
{
    private CameraRig _cameraRig;
    private CameraRig.CameraState _currentState;

    public void Init()
    {
        _cameraRig = Object.FindAnyObjectByType<CameraRig>();

        if (_cameraRig != null)
        {
            ChangeState(CameraRig.CameraState.ThirdPerson);
        }

        Debug.Log("Camera Manager Initialized");
    }

    public void OnUpdate()
    {       
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleCamera();
        }
    }

    public void ChangeState(CameraRig.CameraState newState)
    {
        _currentState = newState;
        _cameraRig?.SetState(newState);
        Debug.Log($"Camera Switched to: {newState}");
    }

    private void ToggleCamera()
    {
        int next = ((int)_currentState + 1) % 2;
        ChangeState((CameraRig.CameraState)next);
    }

    public void UpdateCameraSetting(float currentCatSize)
    {
        _cameraRig.UpdateCameraSettings(currentCatSize);
    }
}