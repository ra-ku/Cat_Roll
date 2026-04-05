using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    public Transform targetBall; 
    public float followDistance = 1.0f; 
    public Transform mainCameraTransform;

    private CharacterController _controller;
    private Transform _mainCameraTransform;
    private Vector3 _velocity; 

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        HandleMovement();
        //ApplyGravity();
    }

    private void HandleMovement()
    {
        var inputManager = Managers.Instance.Get<InputManager>();
        if (inputManager == null) return;

        Vector2 input = inputManager.MoveInput;

        if (input.sqrMagnitude <= 0.01f) return;

        Vector3 camForward = _mainCameraTransform.forward;
        Vector3 camRight = _mainCameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * input.y + camRight * input.x).normalized;

        _controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        //if (moveDirection != Vector3.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}
    }

    private void ApplyGravity()
    {
        // 땅에 닿아있고 떨어지는 중이라면 속도 초기화
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 바닥에 확실히 밀착시키기 위해 작은 음수값 부여
        }

        // 중력 적용 (y축 속도 지속 감소)
        _velocity.y += gravity * Time.deltaTime;

        // 수직 이동 적용
        _controller.Move(_velocity * Time.deltaTime);
    }
}