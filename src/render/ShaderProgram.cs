// render/ShaderProgram.cs
namespace Unminal.Render.ShaderProgram;

[SupportedOSPlatform("windows")]
public class Shader: IDisposable
{
    int _handle;

    int vertexShader;
    int fragmentShader;
    string readedVertexShader;
    string readedFragmentShader;

    Dictionary<string, int> _uniformLocations;

    public Shader(string vertPath, string fragPath)
    {
        if (!File.Exists(vertPath) || !File.Exists(fragPath)){
            Console.WriteLine("[ShaderProgram.cs:19]: Error: Vertex and fragment shader file not exist (shaderProgramm.cs:18)"); 
            throw new FileNotFoundException();
        }

        readedVertexShader = File.ReadAllText(vertPath);
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, readedVertexShader);

        readedFragmentShader = File.ReadAllText(fragPath);
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, readedFragmentShader);

        GL.CompileShader(vertexShader);
        GL.CompileShader(fragmentShader);

        CheckShaderCompilation(vertexShader);
        CheckShaderCompilation(fragmentShader);

        _handle = GL.CreateProgram();

        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);

        GL.LinkProgram(_handle);
        CheckShaderLink();

        _uniformLocations = new Dictionary<string, int>();

        _uniformLocations["model"] = GL.GetUniformLocation(_handle, "model");
        _uniformLocations["view"] = GL.GetUniformLocation(_handle, "view");
        _uniformLocations["projection"] = GL.GetUniformLocation(_handle, "projection");

        _uniformLocations["lightPos"] = GL.GetUniformLocation(_handle, "lightPos");
        _uniformLocations["lightColor"] = GL.GetUniformLocation(_handle, "lightColor");
        _uniformLocations["objectColor"] = GL.GetUniformLocation(_handle, "objectColor");

        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use(){GL.UseProgram(_handle);}

    public void CheckShaderCompilation(int shaderId)
    {   
        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
            throw new Exception($"[ShaderProgram] Shader Error: {GL.GetShaderInfoLog(shaderId)}");
    }

    public void CheckShaderLink()
    {
        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
            throw new Exception($"[ShaderProgram] Link Error: {GL.GetProgramInfoLog(_handle)}");
    }

    public void SetVector3(string name, Vector3 vector){GL.Uniform3(_uniformLocations[name], vector);}
    public void SetMatrix4(string name, Matrix4 matrix){GL.UniformMatrix4(_uniformLocations[name], false, ref matrix);}

    public void Dispose()
    {
       GL.DeleteProgram(_handle); 
    }
}