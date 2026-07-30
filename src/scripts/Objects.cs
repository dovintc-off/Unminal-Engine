namespace Unminal.Script;

[SupportedOSPlatform("windows")]
public class Objects {
    public static  List<GameObject> LoadObjects(List<GameObject> _objects){
        Scene.circle = new Circle(new Vector2(400, 400), 64, new Vector4(0f, 1f, 0f, 0.7f), 64);

        Scene.cube1 = new GameObject(GetPath.GetCorrectPath("obj:/cube.obj")); 
        Scene.cube1.Position = new Vector3(0, 0, -40);
        Scene.cube1.Scale = new Vector3(4f);
        Scene.cube1.Color = Colors.CornflowerBlue;
        _objects.Add(Scene.cube1);

        Scene.cube2 = new GameObject(GetPath.GetCorrectPath("obj:/cube.obj")); 
        Scene.cube2.Position = new Vector3(0, 8, -40);
        Scene.cube2.Scale = new Vector3(4f);
        Scene.cube2.Color = Colors.Silver;
        _objects.Add(Scene.cube2);

        Scene.teapot1 = new GameObject(GetPath.GetCorrectPath("obj:/teapot.obj")); 
        Scene.teapot1.Position = new Vector3(-15, 0, -40);
        Scene.teapot1.Scale = new Vector3(0.2f);
        Scene.teapot1.Color = Colors.Green;
        _objects.Add(Scene.teapot1);
        
        return _objects;
    }
}

public static class Scene {
    public static Circle? circle;
    public static GameObject? cube1;
    public static GameObject? cube2;
    public static GameObject? teapot1;
}