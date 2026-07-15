using Unminal.Main;

namespace Unminal.Script.Core;

[SupportedOSPlatform("windows")]
public abstract class BaseGame
{
    public Camera? ActiveCamera { get; set; }

    public virtual void Load(Matrix4 initialProjection) 
    {
        if (ActiveCamera == null)
            ActiveCamera = new Camera(new Vector3(0, 5, 10), -90.0f, 0.0f);
    }
    public virtual void Update(FrameUpdateVars FUV) 
    {
        ActiveCamera?.ProcessInput(FUV.Keyboard, FUV.DeltaTime);
    }
    public virtual void Draw(Matrix4 projection){}
}