namespace meshing;

public class Camera
{
    public Vector3 Position { get; private set; }
    public float Pitch { get; private set; } = -MathF.PI / 6;
    public float Yaw { get; private set; } = MathF.PI / 4;
    public Vector2 LastMousePosition { get; set; }
    
    private readonly float _fieldOfView = 50.0f / 180.0f * MathF.PI;
    private readonly float _nearPlane = 1.0f;
    private readonly float _farPlane = 256.0f;
    
    public float AspectRatio { get; set; }

    public Camera(Vector3 initialPosition)
    {
        Position = initialPosition;
    }

    public void UpdateFromMouse(Vector2 currentMousePosition)
    {
        var diff = LastMousePosition - currentMousePosition;
        Yaw -= diff.X * 0.003f;
        Pitch += diff.Y * 0.003f;
        Pitch = Math.Clamp(Pitch, -MathF.PI/2 + 0.01f, MathF.PI/2 - 0.01f);
        LastMousePosition = currentMousePosition;
    }

    public void Move(Vector3 direction, float speed)
    {
        Position += direction * speed;
    }

    public Matrix4x4 GetViewMatrix()
    {
        return Helper.CreateFPSView(Position, Pitch, Yaw);
    }

    public Matrix4x4 GetProjectionMatrix()
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(_fieldOfView, AspectRatio, _nearPlane, _farPlane);
    }

    public Matrix4x4 GetViewProjectionMatrix()
    {
        return GetViewMatrix() * GetProjectionMatrix();
    }
}