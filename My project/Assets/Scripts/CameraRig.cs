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
    public CinemachineCamera freeLookCam;

    public void SetState(CameraState state)
    {
        firstPersonCam.Priority = 10;
        thirdPersonCam.Priority = 10;

        switch (state)
        {
            case CameraState.FirstPerson: firstPersonCam.Priority = 20; break;
            case CameraState.ThirdPerson: thirdPersonCam.Priority = 20; break;
        }
    }
}