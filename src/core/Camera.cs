// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.PlayerCamera;

[SupportedOSPlatform("windows")]
public class Camera {
    public Vector3 Front => _front;
    public Vector3 Position { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    public float[] limitationFOV {get; set;} = {30.0f, 120.0f};
    private float _fov = MathHelper.PiOver4;
    public float FOV
    { 
        get => _fov; 
        set 
        {
            _fov = MathHelper.Clamp(value, 
                MathHelper.DegreesToRadians(limitationFOV[0]), 
                MathHelper.DegreesToRadians(limitationFOV[1])
            );
        }
    }

    private Vector3 _front;
    private Vector3 _up;
    private Vector3 _right;
    private readonly Vector3 _worldUp = Vector3.UnitY;

    public float MoveSpeed { get; set; } = 5.0f;
    public float MouseSensitivity { get; set; } = 0.1f;

    /// <summary>
    /// Initializes a new instance of the Camera class with a position and optional orientation.
    /// </summary>
    /// <param name="position">The initial position of the camera in 3D space.</param>
    /// <param name="yaw">The initial horizontal rotation (yaw) of the camera.</param>
    /// <param name="pitch">The initial vertical rotation (pitch) of the camera.</param>
    public Camera(Vector3 position, float yaw = -90.0f, float pitch = 0.0f)
    {
        Position = position;
        Yaw = yaw;
        Pitch = pitch;
        UpdateVectors();
    }

    public Matrix4 GetViewMatrix() {
        return Matrix4.LookAt(Position, Position + _front, _up);
    }

    /// <summary>
    /// Updates the camera's directional vectors (Front, Right, and Up) based on the current Yaw and Pitch values.
    /// </summary>
    private void UpdateVectors()
    {
        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front.Y = -MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        
        _front = Vector3.Normalize(front);
        _right = Vector3.Normalize(Vector3.Cross(_front, _worldUp));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }
    
    /// <summary>
    /// Handles keyboard input, makes it possible to control the camera.
    /// </summary>
    /// <param name="input">The current state of the keyboard.</param>
    /// <param name="deltaTime">The time elapsed since the previous frame (for smooth animation).</param>
    public void ProcessInput(KeyboardState? input, float deltaTime) {
        if (input == null) {
            Console.WriteLine("[#red] Input is null");
            return;
        }
        float velocity = MoveSpeed * deltaTime;

        if (input.IsKeyDown(Keys.W)) Position += _front * velocity;
        if (input.IsKeyDown(Keys.S)) Position -= _front * velocity;
        if (input.IsKeyDown(Keys.A)) Position -= _right * velocity;
        if (input.IsKeyDown(Keys.D)) Position += _right * velocity;

        if (input.IsKeyDown(Keys.Space)) Position += Vector3.UnitY * velocity;
        if (input.IsKeyDown(Keys.LeftShift)) Position -= Vector3.UnitY * velocity;
    }

    /// <summary>
    /// Processes mouse movement to rotate the camera around the Yaw and Pitch axes.
    /// Clamps the Pitch angle between -89 and 89 degrees to prevent screen flipping.
    /// </summary>
    /// <param name="xOffset">The horizontal movement offset of the mouse.</param>
    /// <param name="yOffset">The vertical movement offset of the mouse.</param>
    public void ProcessMouseMovement(float xOffset, float yOffset)
    {
        xOffset *= MouseSensitivity;
        yOffset *= MouseSensitivity;

        Yaw += xOffset;
        Pitch += yOffset;

        if (Pitch > 89.0f) Pitch = 89.0f;
        if (Pitch < -89.0f) Pitch = -89.0f;

        UpdateVectors();
    }
    
    public void ProcessMouseScroll(float yOffset)
    {
        FOV -= (yOffset / 120.0f) * 4f;
    }
}
