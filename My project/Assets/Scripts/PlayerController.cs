using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;        
    public float inertiaSpeed = 5f;      
    public float turnSpeed = 150f;     

    [Header("Spin Settings")]
    public float spinMultiplier = 50f;   
    public float recoverySpeed = 5f; 

    [Tooltip("스킬 연출 등에서 기본 스핀을 잠시 끄고 싶을 때 false로 설정")]
    public bool isModelSpinningEnabled = true;

    [Header("Impact & Physics Settings")]
    public float impactFriction = 5f;

    private CharacterController _controller;
    public GameObject _playerModel;

    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _impactVelocity = Vector3.zero;
    private Quaternion origin_Rotation;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        origin_Rotation = Quaternion.identity;
    }

    private void Update()
    {
        HandleMovement();
    }

    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        _impactVelocity += direction * force;
    }

    private void HandleMovement()
    {
        var inputManager = Managers.Instance.Get<InputManager>();
        if (inputManager == null) return;

        Vector2 input = inputManager.MoveInput;

        if (Mathf.Abs(input.x) > 0.01f)
        {
            transform.Rotate(Vector3.up, input.x * turnSpeed * Time.deltaTime);
        }

        Vector3 inputDirection = transform.forward * input.y;

        Vector3 targetVelocity = inputDirection * maxSpeed;
        _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, inertiaSpeed * Time.deltaTime);

        if (_impactVelocity.magnitude > 0.2f)
        {
            _impactVelocity = Vector3.Lerp(_impactVelocity, Vector3.zero, impactFriction * Time.deltaTime);
        }
        else
        {
            _impactVelocity = Vector3.zero;
        }

        Vector3 finalVelocity = _currentVelocity + _impactVelocity;
        _controller.Move(finalVelocity * Time.deltaTime);

        float currentSpeed = _currentVelocity.magnitude;

        if (currentSpeed > 0.1f)
        {
            if (_playerModel != null && isModelSpinningEnabled)
            {        
                float moveDirectionSign = input.y < 0 ? -1f : 1f;
                float spinAmount = currentSpeed * spinMultiplier * moveDirectionSign * Time.deltaTime;

                _playerModel.transform.Rotate(Vector3.right, spinAmount, Space.Self);
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