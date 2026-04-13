using System; // Action을 쓰기 위해 필수!
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : IManager, IUpdater
{
    public Vector2 MoveInput { get; private set; }

    public event Action OnDashEvent;
    public event Action OnStartGameEvent;

    private Locomotion_Actions _locomotion_Actions;
    private GameFlow_Actions _gameFlow_Actions;

    private InputAction _mouseRightAction;
    private InputAction _moveAction;



    public void Init()
    {
        _locomotion_Actions = new Locomotion_Actions();
        _moveAction = _locomotion_Actions.Move.MoveAction;
        _mouseRightAction = _locomotion_Actions.Move.MoveSkill_Rush;

        _mouseRightAction.performed += OnDashInput;

        _gameFlow_Actions = new GameFlow_Actions();
        _gameFlow_Actions.StartGame.GameStart.performed += OnStartGameInput;

        _gameFlow_Actions.StartGame.Enable();
        _locomotion_Actions.Move.Enable();

        Debug.Log("Input Manager Initialized");
    }

    public void OnUpdate()
    {
        if (_locomotion_Actions == null) return;

        MoveInput = _moveAction.ReadValue<Vector2>();
    }

    private void OnDashInput(InputAction.CallbackContext context)
    {
        OnDashEvent?.Invoke();

        Debug.Log("Mouse Right Button Pressed & Event Fired!");
    }

    private void OnStartGameInput(InputAction.CallbackContext context)
    {
        var gm = Managers.Instance.Get<GameManager>();

        if(gm._state != GameManager.GameState.MainMenu)
        {
            Debug.Log("Start Game Input Detected but Game is not in Main Menu State. Ignoring input.");
            return;
        }

        OnStartGameEvent?.Invoke();
        Debug.Log("Start Game Input Detected & Event Fired!");
    }

    public void Dispose()
    {
        if (_locomotion_Actions != null)
        {
            _locomotion_Actions.Move.Disable();
            _mouseRightAction.performed -= OnDashInput;
            _locomotion_Actions.Dispose();
            _locomotion_Actions = null;
        }

        if(_gameFlow_Actions != null)
        {
            _gameFlow_Actions.StartGame.Disable();
            _gameFlow_Actions.StartGame.GameStart.performed -= OnStartGameInput;
            _gameFlow_Actions.Dispose();
            _gameFlow_Actions = null;
        }
    }
}