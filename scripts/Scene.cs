namespace Dov1ntc.MyGameIn3d;

using Unminal.Render.Objects;
using Unminal.Render.Primitive._2D;
using Unminal.UI.ButtonObject;
using Unminal.UI.Factory;
using Unminal.Utils.Colors;

[SupportedOSPlatform("windows")]
public class Scene {
    public static Circle? circle;
    public static GameObject? cube;
    public static GameObject? node;
    public static GameObject? teapot;
    public static Button? btn1;

    public static List<GameObject> LoadScene(List<GameObject> _objects){
        circle = new Circle(new Vector2(400, 400), 64, new Vector4(0f, 1f, 0f, 0.7f), 64);

        cube = new GameObject(GetPath.GetCorrectPath("scripts/objects/cube.obj")); 
        cube.Position = new Vector3(15, 0, -40);
        cube.Scale = new Vector3(1f);
        cube.Color = Colors.CornflowerBlue;
        _objects.Add(cube);

        node = new GameObject(GetPath.GetCorrectPath("scripts/objects/power_node.obj")); 
        node.Position = new Vector3(0, -5, -40);
        node.Scale = new Vector3(0.4f);
        node.Color = Colors.Silver;
        _objects.Add(node);

        teapot = new GameObject(GetPath.GetCorrectPath("scripts/objects/teapot.obj")); 
        teapot.Position = new Vector3(-15, 0, -40);
        teapot.Scale = new Vector3(0.2f);
        teapot.Color = Colors.Green;
        _objects.Add(teapot);
    
        btn1 = UIFactory.CreateButton(
            pos:new Vector2(190f, 190f), 
            scale:new Vector2(120f, 50f), 
            rotation:0f,
            color:Colors.Blue,
            dimmingMultiplier:0.4f
        );

        return _objects;
    }
}