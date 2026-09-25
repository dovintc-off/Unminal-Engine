namespace Dovintc.MyGameIn3d;

using Unminal.Core.PlayerCamera;
using Unminal.Core.Scripting.Script;
using Unminal.Render.Light;
using Unminal.Render.Objects;

[SupportedOSPlatform("windows")]
[Script]
public class Game : Script {
    private readonly List<GameObject> _spawnedCubes = new();

    private const float CubeSpawnInterval = 0.25f;
    private const float MinCubeSpawnRadius = 8.0f;
    private const float MaxCubeSpawnRadius = 18.0f;
    private const float MinCubeDistance = 4.0f;
    private const float CubeHeightRange = 3.0f;
    private const float CubeScaleMin = 0.5f;
    private const float CubeScaleMax = 2.0f;

    private const float MaxLightIntensity = 10.0f;
    private const float MinLightIntensity = 0.0f;
    private const float LightFadeInterval = 1.0f;

    private float _cubeSpawnTimer;
    private Vector3 _lastPlayerPosition;
    private bool _hasPreviousPlayerPosition;

    private float _lightFadeTimer;
    private float _lightIntensity;
    private Vector3 _lightPosition;
    private Vector3 _lightColor;

    public override void Load(Matrix4 initialProjection) {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);

        _lastPlayerPosition = ActiveCamera.Position;
        _hasPreviousPlayerPosition = true;
        _cubeSpawnTimer = 0.0f;

        _lightFadeTimer = 0.0f;
        _lightIntensity = MaxLightIntensity;
        _lightPosition = ActiveCamera.Position;
        _lightColor = CreateLightColor();

        Engine.LightManager?.ClearLights();
        _spawnedCubes.Clear();

        // Стартовый свет находится прямо в игроке и имеет максимальную силу.
        RecreateLight();
    }

    public override void Update() {
        base.Update();

        Camera? camera = Engine.Player.CameraObj ?? ActiveCamera;
        if (camera == null) return;

        Vector3 playerPosition = camera.Position;

        bool isMoving = _hasPreviousPlayerPosition &&
                        (playerPosition - _lastPlayerPosition).LengthSquared > 0.000001f;

        if (isMoving) {
            _cubeSpawnTimer += Engine.DeltaTime;

            if (_cubeSpawnTimer >= CubeSpawnInterval) {
                _cubeSpawnTimer = 0.0f;
                SpawnCubeAroundPlayer(playerPosition);
            }
        } else {
            _cubeSpawnTimer = 0.0f;
        }

        _lastPlayerPosition = playerPosition;
        _hasPreviousPlayerPosition = true;

        // Каждую секунду заменяем текущий свет на новый с меньшей силой.
        _lightFadeTimer += Engine.DeltaTime;

        if (_lightFadeTimer >= LightFadeInterval) {
            _lightFadeTimer -= LightFadeInterval;

            _lightIntensity = MathF.Max(
                MinLightIntensity,
                _lightIntensity - 1.0f
            );

            RecreateLight();
        }

        // E полностью восстанавливает свет до 10 и переносит его
        // на 40 единиц вперёд от игрока.
        if (Engine.CurrentKeyboard?.IsKeyReleased(Keys.E) == true) {
            _lightIntensity = MaxLightIntensity;
            _lightFadeTimer = 0.0f;
            _lightPosition = camera.Position + camera.Front * 40.0f;
            _lightColor = CreateLightColor();

            RecreateLight();
        }
    }

    public override void Draw() {
        foreach (GameObject cube in _spawnedCubes) {
            cube.Draw();
        }
    }

    private void SpawnCubeAroundPlayer(Vector3 playerPosition) {
        const int maxAttempts = 12;

        for (int attempt = 0; attempt < maxAttempts; attempt++) {
            float angle = Random.Shared.NextSingle() * MathF.Tau;
            float radius = Random.Shared.NextSingle() *
                           (MaxCubeSpawnRadius - MinCubeSpawnRadius) +
                           MinCubeSpawnRadius;

            Vector3 position = playerPosition + new Vector3(
                MathF.Cos(angle) * radius,
                (Random.Shared.NextSingle() * 2.0f - 1.0f) * CubeHeightRange,
                MathF.Sin(angle) * radius
            );

            if ((position - playerPosition).LengthSquared < MinCubeDistance * MinCubeDistance)
                continue;

            bool overlapsExistingCube = false;

            foreach (GameObject existingCube in _spawnedCubes) {
                if ((position - existingCube.Position).LengthSquared < 4.0f * 4.0f) {
                    overlapsExistingCube = true;
                    break;
                }
            }

            if (overlapsExistingCube)
                continue;

            GameObject cube = new GameObject(
                GetPath.GetCorrectPath("Assets/objects/cube.obj")
            );

            cube.Position = position;

            float scale = Random.Shared.NextSingle() *
                          (CubeScaleMax - CubeScaleMin) +
                          CubeScaleMin;

            cube.Scale = new Vector3(scale);
            cube.Color = CreateDarkRandomColor();

            cube.Orientation = Quaternion.FromEulerAngles(
                Random.Shared.NextSingle() * MathF.Tau,
                Random.Shared.NextSingle() * MathF.Tau,
                Random.Shared.NextSingle() * MathF.Tau
            );

            _spawnedCubes.Add(cube);
            return;
        }
    }

    private static Vector3 CreateDarkRandomColor() {
        float red = Random.Shared.NextSingle() * 0.35f + 0.10f;
        float green = Random.Shared.NextSingle() * 0.35f + 0.10f;
        float blue = Random.Shared.NextSingle() * 0.35f + 0.10f;

        return new Vector3(red, green, blue);
    }

    private void RecreateLight() {
        // Костыль намеренный: удаляем старый свет и создаём новый.
        Engine.LightManager?.ClearLights();

        LightData light = new LightData(
            _lightPosition,
            _lightColor,
            Math.Clamp(_lightIntensity, MinLightIntensity, MaxLightIntensity)
        );

        Engine.LightManager?.AddLight(light);
    }

    private static Vector3 CreateLightColor() {
        float red = Random.Shared.NextSingle() * 0.5f + 0.5f;
        float green = Random.Shared.NextSingle() * 0.5f + 0.5f;
        float blue = Random.Shared.NextSingle() * 0.5f + 0.5f;

        return new Vector3(red, green, blue);
    }

    public override void Unload() {
        foreach (GameObject cube in _spawnedCubes) {
            cube.Dispose();
        }

        _spawnedCubes.Clear();

        Engine.LightManager?.ClearLights();

        base.Unload();
    }
}
