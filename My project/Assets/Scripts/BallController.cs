using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public float rollSpeed = 10f;
    private Rigidbody _rb;
    private Transform _mainCameraTransform;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCameraTransform = Camera.main.transform;

        _rb.maxAngularVelocity = 20f;
    }

    void FixedUpdate()
    {
        var inputManager = Managers.Instance.Get<InputManager>();
        if (inputManager == null) return;

        Vector2 input = inputManager.MoveInput;
        if (input.sqrMagnitude <= 0.01f) return;

        // 카메라 방향 기준 계산
        Vector3 camForward = _mainCameraTransform.forward;
        Vector3 camRight = _mainCameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * input.y + camRight * input.x).normalized;

        // 공을 굴리는 핵심 원리: 이동 방향의 '직각' 방향으로 회전력(Torque)
        Vector3 torqueDirection = new Vector3(moveDirection.z, 0, -moveDirection.x);
        _rb.AddTorque(torqueDirection * rollSpeed, ForceMode.Acceleration);
    }
}