using Silk.NET.Input;

namespace meshing;

public class CameraController(Camera camera, IKeyboard keyboard, IMouse mouse)
{
    private readonly Camera _camera = camera;
    private readonly IKeyboard _keyboard = keyboard;
    private readonly IMouse _mouse = mouse;
    private const float MovementSpeed = 0.25f;

    public void Update()
    {
        HandleMouseMovement();
        HandleKeyboardMovement();
        HandleEscapeKey();
    }

    private void HandleMouseMovement()
    {
        _camera.UpdateFromMouse(_mouse.Position);
    }

    private void HandleKeyboardMovement()
    {
        if (_keyboard.IsKeyPressed(Key.W))
            _camera.Move(Helper.FromPitchYaw(_camera.Pitch, _camera.Yaw), MovementSpeed);
        else if (_keyboard.IsKeyPressed(Key.S))
            _camera.Move(-Helper.FromPitchYaw(_camera.Pitch, _camera.Yaw), MovementSpeed);

        if (_keyboard.IsKeyPressed(Key.A))
            _camera.Move(Helper.FromPitchYaw(0, _camera.Yaw - MathF.PI / 2), MovementSpeed);
        else if (_keyboard.IsKeyPressed(Key.D))
            _camera.Move(Helper.FromPitchYaw(0, _camera.Yaw + MathF.PI / 2), MovementSpeed);

        if (_keyboard.IsKeyPressed(Key.E))
            _camera.Move(Helper.FromPitchYaw(MathF.PI / 2, 0), MovementSpeed);
        else if (_keyboard.IsKeyPressed(Key.Q))
            _camera.Move(Helper.FromPitchYaw(-MathF.PI / 2, 0), MovementSpeed);
    }

    private void HandleEscapeKey()
    {
        if (_keyboard.IsKeyPressed(Key.Escape))
            // You'll need to inject IWindow or handle this differently
            System.Environment.Exit(0); // Temporary solution
    }
}