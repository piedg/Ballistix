using System;
using Unity.Mathematics;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputActions _actions;

    public event Action OnJump;
    public event Action OnPause;

    public static InputManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _actions = new InputActions();

        _actions.Player.Enable();
        
        _actions.UI.Enable();

        _actions.Player.Jump.performed += Jump_performed;
        _actions.UI.Pause.performed += Pause_performed;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        var inputVector = _actions.Player.Move.ReadValue<Vector2>();

        inputVector = new(math.round(inputVector.x), inputVector.y);

        inputVector = math.normalizesafe(inputVector);

        return inputVector;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJump?.Invoke();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPause?.Invoke();
    }
}