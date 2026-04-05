using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : IManager, IUpdater
{
    public Vector2 MoveInput { get; private set; }
    public bool IsMouseLeftPressed { get; private set; }

    private Locomotion_Actions _actions;
    private InputAction _moveAction;

    public void Init()
    {
        _actions = new Locomotion_Actions();
        _moveAction = _actions.Move.MoveAction;

        //_actions.Move.MouseLeftMove.started += ctx => IsMouseLeftPressed = true;
        //_actions.Move.MouseLeftMove.canceled += ctx => IsMouseLeftPressed = false;

        _actions.Move.Enable();

        Debug.Log("Input Manager Initialized");
    }

    public void OnUpdate()
    {
        if (_actions == null) return;

        MoveInput = _moveAction.ReadValue<Vector2>();

        if (MoveInput.sqrMagnitude > 0)
        {
            Debug.Log($"입력 중: {MoveInput}");
        }

    }

    public void Dispose()
    {
        if (_actions != null)
        {
            _actions.Move.Disable();
            _actions.Dispose();
            _actions = null;
        }
    }
}