global using static meshing.Globals;
global using System;
global using System.Numerics;
global using System.Runtime.InteropServices;
global using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Threading.Tasks;
namespace meshing;


// This allows every file to call Gl.DoStuff()
public static class Globals
{
    public static GL Gl;

    public static void Assert(bool condition) => Debug.Assert(condition);
    public static void AssertFalse() => Debug.Assert(false);
}


internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Create and run the client
            var client = new Client();
            client.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex}");
#if DEBUG
            Debugger.Break();
#endif
        }
    }
}
