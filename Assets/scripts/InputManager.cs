#nullable enable

using UnityEngine;

static class InputManager {
    private static readonly InputSystem_Actions inputActions = new();

    public static void Initialize() {
        inputActions.Enable();
    }

    public static bool GetJump() {
        bool pressed = inputActions.Player.Jump.WasPressedThisFrame();

        inputActions.Player.Jump.Reset();

        return pressed;
    }

    public static bool GetGrab() {
        return inputActions.Player.Grab.IsPressed();
    }

    public static Vector2 GetMove() {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }

    public static Vector2 GetLook() {
        return inputActions.Player.Look.ReadValue<Vector2>();
    }

    public static bool GetPrevious() {
        bool pressed = inputActions.Player.Previous.WasPressedThisFrame();

        inputActions.Player.Previous.Reset();

        return pressed;
    }

    public static bool GetNext() {
        bool pressed = inputActions.Player.Next.WasPressedThisFrame();

        inputActions.Player.Next.Reset();

        return pressed;
    }
}
