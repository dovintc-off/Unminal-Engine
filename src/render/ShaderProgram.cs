// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Render.ShaderProgram;

[SupportedOSPlatform("windows")]
public class Shader: IDisposable {
    int _handle;
    int vertexShader;
    int fragmentShader;
    string readedVertexShader;
    string readedFragmentShader;
    Dictionary<string, int> _uniformLocations;

    public int Handle => _handle;

    public Shader(string vertPath, string fragPath) {
        if (!File.Exists(vertPath) || !File.Exists(fragPath)){
            Log.Create(Log.LogType.ERROR, "Vertex or fragment shader file not found", CrashGame: true, ""); 
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
        _uniformLocations["objectColor"] = GL.GetUniformLocation(_handle, "objectColor");
        _uniformLocations["viewPos"] = GL.GetUniformLocation(_handle, "viewPos");
        
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

    public void SetVector3(string name, Vector3 vector)
    {
        if (_uniformLocations.TryGetValue(name, out int loc) && loc != -1)
        {
            GL.Uniform3(loc, vector);
        }
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        if (_uniformLocations.TryGetValue(name, out int loc) && loc != -1)
        {
            GL.UniformMatrix4(loc, false, ref matrix);
        }
    }

    public void Dispose()
    {
       GL.DeleteProgram(_handle); 
    }
}
