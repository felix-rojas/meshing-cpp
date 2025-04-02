using Silk.NET.Input;
using Silk.NET.Windowing;
namespace meshing;
using ImGuiNET;

public unsafe partial class Client
{
    // Add these with your other fields
    private double _frameTimeAccumulator;
    private int _frameCount;
    private double _currentFps;
    private double _currentFrameTime;
    private readonly System.Text.StringBuilder _fpsTextBuilder = new();
    private readonly IWindow window;
    private readonly IWindow window2;
    private Camera camera;
    private CameraController cameraController;
    private MapRenderer mapRenderer;
    private IKeyboard keyboard;
    private IMouse mouse;
    private IInputContext inputContext;


    public Client()
    {
        var options = WindowOptions.Default;
        options.API = new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(3, 3));
        options.Position = new(200, 200);
        options.PreferredDepthBufferBits = 32;

        window = Window.Create(options);

        window.Load += OnWindowLoad;
        window.Render += OnRender;
        window.Update += OnUpdate;

        window.Size = new(1600, 1000);
        window.FramesPerSecond = 144;
        window.UpdatesPerSecond = 144;
        window.VSync = false;

        window.Initialize();

        window2 = Window.Create(options);
        window2.Load += OnWindow2Load;
        window2.Render += OnRender2;
        window2.Update += OnUpdate;
        window2.Size = new(600, 600);
        window2.FramesPerSecond = 60;
        window2.UpdatesPerSecond = 60;
        window2.VSync = false;
        window2.Initialize();
    }

    private void OnWindowLoad()
    {
        Gl = window.CreateOpenGL();
        OnDidCreateOpenGLContext();

        inputContext = window.CreateInput();
        keyboard = inputContext.Keyboards[0];
        mouse = inputContext.Mice[0];
        mouse.DoubleClickTime = 1;

        // Initialize camera with proper starting position
        camera = new Camera(new Vector3(64, 64, 64));
        cameraController = new CameraController(camera, keyboard, mouse);

        InitializeFpsCounter();

    }

        private void OnWindow2Load()
    {
        Gl = window.CreateOpenGL();
        OnDidCreateOpenGLContext();

        InitializeFpsCounter();
    }

    private void InitializeFpsCounter()
    {
        _currentFps = 0;
        _frameCount = 0;
        _frameTimeAccumulator = 0;
    }

    private void UpdateFpsCounter(double deltaTime)
    {
        _frameTimeAccumulator += deltaTime;
        _frameCount++;
        _currentFrameTime = deltaTime;

        if (_frameTimeAccumulator >= 1.00)
        {
            _currentFps = _frameCount / _frameTimeAccumulator;
            _frameCount = 0;
            _frameTimeAccumulator = 0;
            _fpsTextBuilder.Clear();
            _fpsTextBuilder.AppendFormat("FPS: {0:0.0} ({1:0.00}ms)", _currentFps, _currentFrameTime * 1000);
            Console.WriteLine(_fpsTextBuilder);
        }
    }


    private void OnUpdate(double deltaTime)
    {
        camera.AspectRatio = window.Size.X / (float)window.Size.Y;
        cameraController.Update();
    }

    private void OnRender(double deltaTime)
    {
        UpdateFpsCounter(deltaTime);

        PreRenderSetup();

        VoxelShader.UseProgram();
        VoxelShader.mvp.Set(camera.GetViewProjectionMatrix());
        VoxelShader.showWireframe.Set(keyboard.IsKeyPressed(Key.Space));

        mapRenderer.cameraPitch = camera.Pitch;
        mapRenderer.cameraYaw = camera.Yaw;
        mapRenderer.Render();
    }

        private void OnRender2(double deltaTime)
    {
        UpdateFpsCounter(deltaTime);

        PreRenderSetup();
    }

    private void OnDidCreateOpenGLContext()
    {
        var major = Gl.GetInteger(GetPName.MajorVersion);
        var minor = Gl.GetInteger(GetPName.MinorVersion);
        Console.WriteLine($"OpenGL Version: {major * 10 + minor}");

        mapRenderer = new(128, 64, 128);

#if DEBUG
        debugDelegate = DebugCallback;
        Gl.Enable(EnableCap.DebugOutput);
        Gl.Enable(EnableCap.DebugOutputSynchronous);
        Gl.DebugMessageCallback(debugDelegate, null);
#endif
    }

    // Rest of your methods (PreRenderSetup, etc.) remain the same

    public void PreRenderSetup()
    {
        // Prepare rendering
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        Gl.Enable(EnableCap.DepthTest);
        Gl.Disable(EnableCap.Blend);
        Gl.Disable(EnableCap.StencilTest);
        Gl.Enable(EnableCap.CullFace);
        Gl.FrontFace(FrontFaceDirection.CW);

        // Clear everything
        Gl.ClearDepth(1.0f);
        Gl.DepthFunc(DepthFunction.Less);

        Gl.ColorMask(true, true, true, true);
        Gl.DepthMask(true);

        Gl.ClearColor(0, 0, 0, 0);
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        // Set the viewport to the window size
        Gl.Viewport(0, 0, (uint)window.Size.X, (uint)window.Size.Y);
    }
    public void Run()
    {
        window.Run();
    }

#if DEBUG
    static DebugProc debugDelegate;

    static unsafe void DebugCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint messageInt, nint userParam)
    {
        var message = Marshal.PtrToStringAnsi(messageInt);

        if (message == "Pixel-path performance warning: Pixel transfer is synchronized with 3D rendering.")
            return;

        if (severity == GLEnum.DebugSeverityNotification)
            return;

        if (id == 131185 || id == 131218 || id == 131186)
            return;

        AssertFalse();
        Console.WriteLine(message);
    }
#endif
}