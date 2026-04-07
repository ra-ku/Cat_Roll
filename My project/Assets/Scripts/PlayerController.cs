using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float spinSpeed = 720f;

    public float recoverySpeed = 10f;

    private CharacterController _controller;
    private Transform _mainCameraTransform;
    public GameObject _playerModel;

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
    private void HandleMovement()
    {
        var inputManager = Managers.Instance.Get<InputManager>();
        if (inputManager == null) return;

        Vector2 input = inputManager.MoveInput;

        bool hasInput = input.sqrMagnitude > 0.01f;

        if (hasInput)
        {
            // --- 키를 누르고 있을 때 (이동 및 회전 적용) ---
            Vector3 camForward = _mainCameraTransform.forward;
            Vector3 camRight = _mainCameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = (camForward * input.y + camRight * input.x).normalized;

            _controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            if (_playerModel != null)
            {
                switch (input)
                {
                    case Vector2 v when v.x > 0.1f && v.y > 0.1f: // 우상단 
                        _playerModel.transform.Rotate((Vector3.up + Vector3.right).normalized, spinSpeed * Time.deltaTime);
                        break;
                    case Vector2 v when v.x < -0.1f && v.y < -0.1f: // 좌하단
                        _playerModel.transform.Rotate((Vector3.down + Vector3.left).normalized, spinSpeed * Time.deltaTime);
                        break;
                    case Vector2 v when v.x > 0.1f && v.y < -0.1f: // 우하단
                        _playerModel.transform.Rotate((Vector3.up + Vector3.left).normalized, spinSpeed * Time.deltaTime);
                        break;
                    case Vector2 v when v.x < -0.1f && v.y > 0.1f: // 좌상단
                        _playerModel.transform.Rotate((Vector3.down + Vector3.right).normalized, spinSpeed * Time.deltaTime);
                        break;

                    // --- 2. 상하좌우 단일 입력 ---
                    case Vector2 v when v.x > 0.1f && Mathf.Abs(v.y) <= 0.1f: // 우
                        _playerModel.transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
                        break;
                    case Vector2 v when v.x < -0.1f && Mathf.Abs(v.y) <= 0.1f: // 좌
                        _playerModel.transform.Rotate(Vector3.down, spinSpeed * Time.deltaTime);
                        break;
                    case Vector2 v when v.y > 0.1f && Mathf.Abs(v.x) <= 0.1f: // 상
                        _playerModel.transform.Rotate(Vector3.right, spinSpeed * Time.deltaTime);
                        break;
                    case Vector2 v when v.y < -0.1f && Mathf.Abs(v.x) <= 0.1f: // 하
                        _playerModel.transform.Rotate(Vector3.left, spinSpeed * Time.deltaTime);
                        break;
                }
            }
        }
        else
        {
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