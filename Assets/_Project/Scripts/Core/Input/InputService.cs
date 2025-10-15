using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputService
{
    Vector2 MoveInput { get; }
    bool ConfirmPressed { get; }
    bool CancelPressed { get; }
}

public class InputService : IInputService, System.IDisposable
{
    private readonly GameInput _gameInput;
    private Vector2 _move;
    private bool _confirm;
    private bool _cancel;

    public InputService()
    {
        _gameInput = new GameInput();
        _gameInput.Enable();

        _gameInput.BuildingActions.Move.performed += ctx => _move = ctx.ReadValue<Vector2>();
        _gameInput.BuildingActions.Move.canceled += _ => _move = Vector2.zero;

        _gameInput.BuildingActions.Confirm.performed += _ => _confirm = true;
        _gameInput.BuildingActions.Cancel.performed += _ => _cancel = true;
    }

    public Vector2 MoveInput => _move;
    public bool ConfirmPressed => Consume(ref _confirm);
    public bool CancelPressed => Consume(ref _cancel);

    private bool Consume(ref bool value)
    {
        if (!value) return false;
        value = false;
        return true;
    }

    public void Dispose() => _gameInput.Dispose();
}
