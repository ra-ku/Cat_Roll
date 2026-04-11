using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;          // 최고 이동 속도
    public float inertiaSpeed = 5f;      // 이동 관성 (값이 작을수록 미끄러짐)
    public float rotationSpeed = 10f;    // 방향 전환 시 몸을 돌리는 속도

    [Header("Spin Settings")]
    public float spinMultiplier = 50f;   // 구르는 속도 배율
    public float recoverySpeed = 5f;     // 정지 시 정면으로 돌아오는 속도

    [Tooltip("스킬 연출 등에서 기본 스핀을 잠시 끄고 싶을 때 false로 설정합니다.")]
    public bool isModelSpinningEnabled = true;

    [Header("Impact & Physics Settings")]
    public float impactFriction = 5f;    // 대시나 넉백 후 미끄러지다 멈추는 마찰력

    private CharacterController _controller;
    private Transform _mainCameraTransform;
    public GameObject _playerModel;

    private Vector3 _currentVelocity = Vector3.zero; // 유저 입력에 의한 현재 속도
    private Vector3 _impactVelocity = Vector3.zero;  // 외부 요인(대시, 넉백)에 의한 충격량
    private Quaternion origin_Rotation;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }

        origin_Rotation = Quaternion.identity;
    }

    private void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// 외부 스크립트(SkillController 등)에서 대시나 넉백을 구현할 때 호출합니다.
    /// Rigidbody의 AddForce(Impulse)와 유사한 역할을 합니다.
    /// </summary>
    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        _impactVelocity += direction * force;
    }

    private void HandleMovement()
    {
        // 1. 입력 처리 (InputManager)
        var inputManager = Managers.Instance.Get<InputManager>();
        if (inputManager == null) return;

        Vector2 input = inputManager.MoveInput;

        // 2. 카메라 기준 방향 벡터 계산
        Vector3 camForward = _mainCameraTransform.forward;
        Vector3 camRight = _mainCameraTransform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 inputDirection = (camForward * input.y + camRight * input.x).normalized;

        // --- 🎯 이동 속도 및 충격량(관성/마찰력) 연산 ---

        // 3. 유저 입력에 의한 목표 속도 및 현재 관성 속도 (Lerp)
        Vector3 targetVelocity = inputDirection * maxSpeed;
        _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, inertiaSpeed * Time.deltaTime);

        // 4. 외부 충격량(대시, 넉백) 감쇄 로직
        if (_impactVelocity.magnitude > 0.2f)
        {
            _impactVelocity = Vector3.Lerp(_impactVelocity, Vector3.zero, impactFriction * Time.deltaTime);
        }
        else
        {
            _impactVelocity = Vector3.zero;
        }

        // 5. 최종 이동 적용 (기본 이동 속도 + 순간 충격 속도)
        Vector3 finalVelocity = _currentVelocity + _impactVelocity;
        _controller.Move(finalVelocity * Time.deltaTime);

        // --- 🎯 회전 및 스핀 연산 ---

        float currentSpeed = _currentVelocity.magnitude;

        if (currentSpeed > 0.1f)
        {
            // 6-1. 부모 오브젝트가 이동 방향을 부드럽게 바라보게 회전 (Slerp)
            Quaternion targetRotation = Quaternion.LookRotation(_currentVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 6-2. 자식 모델링 스핀 - 기능이 켜져 있을 때만 속도에 비례해서 구름
            if (_playerModel != null && isModelSpinningEnabled)
            {
                float spinAmount = currentSpeed * spinMultiplier * Time.deltaTime;
                _playerModel.transform.Rotate(Vector3.right, spinAmount, Space.Self);
            }
        }
        else
        {
            // 7. 정지 시 모델링의 스핀만 오뚝이처럼 정면으로 복구
            if (_playerModel != null)
            {
                _playerModel.transform.localRotation = Quaternion.Slerp(
                    _playerModel.transform.localRotation,
                    origin_Rotation,
                    recoverySpeed * Time.deltaTime
                );
            }
        }
    }
}