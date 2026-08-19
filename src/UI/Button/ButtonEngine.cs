// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.UI.ButtonEngine;

using Unminal.Render.Primitive._2D;
using Unminal.UI.EventBus;

[SupportedOSPlatform("windows")]
public class EngineButton : IDisposable {
    public uint ID {get; init;}
    public Square? Bounds {get; set;}
    public List<MouseButton> ForbiddenButtons = new();
    private bool _wasPressedLastFrame;
    private float _heldDuration;

    public EngineButton(uint id, Square? bounds, List<MouseButton> forbiddenButtons) {
        ID = id;
        Bounds = bounds;
        ForbiddenButtons = forbiddenButtons;
    }

    public EngineButton(uint id, Square? bounds)
        : this(id, bounds, new List<MouseButton>())
    {}

    public void Update() {
        MouseState? mouseState = Engine.CurrentMouse;
        float dt = Engine.DeltaTime;
        if (Engine.CurrentCursorState == CursorState.Grabbed) return;
        if (Bounds == null || mouseState == null) return;
        bool isHovered = Bounds.Contains(mouseState.Position);
        (bool isPressed, MouseButton button) = IsPressed(mouseState);
        if (isPressed && ForbiddenButtons.Count != 0 && ForbiddenButtons.Contains(button)) isPressed = false;
        if (isHovered) {
            if (isPressed && !_wasPressedLastFrame) {
                _heldDuration = 0f;
                EventBusUi.Publish(new ButtonPressedEvent {ButtonId = ID});
            } else if (isPressed && _wasPressedLastFrame) {
                _heldDuration += dt;
                EventBusUi.Publish(new ButtonHeldEvent {ButtonId = ID, Duration = _heldDuration});
            } else if (!isPressed && _wasPressedLastFrame) {
                EventBusUi.Publish(new ButtonRelesedEvent {ButtonId = ID});
            }
        } else {
            if (_wasPressedLastFrame) {
                EventBusUi.Publish(new ButtonRelesedEvent {ButtonId = ID});
            }
        }

        _wasPressedLastFrame = isPressed && isHovered;
    }

    private static (bool, MouseButton) IsPressed(MouseState state) {
        if (state[MouseButton.Button1]) return (true, MouseButton.Button1);
        if (state[MouseButton.Button2]) return (true, MouseButton.Button2);
        if (state[MouseButton.Button3]) return (true, MouseButton.Button3);
        if (state[MouseButton.Button4]) return (true, MouseButton.Button4);
        if (state[MouseButton.Button5]) return (true, MouseButton.Button5);
        if (state[MouseButton.Button6]) return (true, MouseButton.Button6);
        if (state[MouseButton.Button7]) return (true, MouseButton.Button7);
        if (state[MouseButton.Button8]) return (true, MouseButton.Button8);
        if (state[MouseButton.Last]) return (true, MouseButton.Last);
        if (state[MouseButton.Left]) return (true, MouseButton.Left);
        if (state[MouseButton.Middle]) return (true, MouseButton.Middle);
        if (state[MouseButton.Right]) return (true, MouseButton.Right);
        return (false, new MouseButton());
    }

    public void Dispose() {
        Bounds?.Dispose();
        ForbiddenButtons.Clear();
    }
}
