// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Script.Core;
using Unminal.Core.PlayerCamera;

[SupportedOSPlatform("windows")]
public abstract class BaseGame {
    public Camera? ActiveCamera { get; set; }
    public virtual void Load(Matrix4 initialProjection)  {
        if (ActiveCamera == null) ActiveCamera = new Camera(new Vector3(0, 5, 10), -90.0f, 0.0f);
    }
    public virtual void Update() { 
        ActiveCamera?.ProcessInput(Engine.CurrentKeyboard, Engine.DeltaTime); 
        if (Engine.Player.CameraObj == null) return;
    }
    public virtual void Draw(){}
    public virtual void Unload(){}
}