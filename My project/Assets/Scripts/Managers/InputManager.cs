using System; // Action을 쓰기 위해 필수!
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : IManager, IUpdater
{
    public Vector2 MoveInput { get; private set; }

    public event Action OnDashEvent;

    private Locomotion_Actions _actions;
    private InputAction _mouseRightAction;
    private InputAction _moveAction;

    public void Init()
    {
        _actions = new Locomotion_Actions();
        _moveAction = _actions.Move.MoveAction;
        _mouseRightAction = _actions.Move.MoveSkill_Rush;

        _mouseRightAction.performed += OnDashInput;
        _actions.Move.Enable();

        Debug.Log("Input Manager Initialized");
    }

    public void OnUpdate()
    {
        if (_actions == null) return;

        MoveInput = _moveAction.ReadValue<Vector2>();
    }

    private void OnDashInput(InputAction.CallbackContext context)
    {
        OnDashEvent?.Invoke();

        Debug.Log("Mouse Right Button Pressed & Event Fired!");
    }

    public void Dispose()
    {
        if (_actions != null)
        {
            _actions.Move.Disable();
            _mouseRightAction.performed -= OnDashInput;
            _actions.Dispose();
            _actions = null;
        }
    }
}