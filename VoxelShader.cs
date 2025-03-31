namespace meshing;

public static class VoxelShader
{
    public static ShaderProgram shader;
    public static bool Active => shader?.Active ?? false;

    public static void Initialise()
    {
        // useful for debugging glsl files
        ReadShadersFromFiles();

        string vertexShader = LoadShaderFile("shaders/vert/voxel.vert");
        string fragmentShader = LoadShaderFile("shaders/frag/voxel.frag");
        
        shader = new(vertexShader, fragmentShader);

        mvp = new(shader, "mvp");
        worldPosition = new(shader, "worldPosition");
        showWireframe = new(shader, "showWireframe");
    }


    public static void UseProgram()
    {
        if (shader == null)
            Initialise();

        shader.UseProgram();
    }

        private static string LoadShaderFile(string path)
    {
        try
        {
            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine($"Shader file not found: {path}");
                return string.Empty;
            }

            return System.IO.File.ReadAllText(path);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading shader file {path}:");
            Console.WriteLine(e.Message);
            return string.Empty;
        }
    }


    private static void ReadShadersFromFiles()
    {
        System.Collections.Generic.IEnumerable<string> fragShaderFiles = System.IO.Directory.EnumerateFiles("shaders/frag", "*.frag", System.IO.SearchOption.AllDirectories);
        System.Collections.Generic.IEnumerable<string> vertexShaderFiles = System.IO.Directory.EnumerateFiles("shaders/vert", "*.vert", System.IO.SearchOption.AllDirectories);
        
        Console.WriteLine("------------------\nLoading shaders...\n------------------");

        foreach (var filePath in fragShaderFiles)
        {
            try
            {
                    Console.WriteLine(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fragment shader could not be read:");
                Console.WriteLine(e.Message);
            }
        }
 
        foreach (var filePath in vertexShaderFiles)
        {
            try
            {
                    Console.WriteLine(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Vertex shader could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine("---------------\nShaders Loaded!\n---------------");
    }

    public static ShaderValue mvp;
    public static ShaderValue worldPosition;
    public static ShaderValue showWireframe;
}
