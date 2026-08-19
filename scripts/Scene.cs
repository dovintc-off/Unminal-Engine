namespace Dov1ntc.MyGameIn3d;

using Unminal.Render.Objects;
using Unminal.Render.Primitive._2D;
using Unminal.UI.ButtonObject;
using Unminal.UI.Factory;
using Unminal.Utils.Colors;

[SupportedOSPlatform("windows")]
public class Scene {
    public static Circle? circle;
    public static GameObject? cube1;
    public static GameObject? cube2;
    public static GameObject? teapot1;
    public static Button? btn1;

    public static List<GameObject> LoadScene(List<GameObject> _objects){
        circle = new Circle(new Vector2(400, 400), 64, new Vector4(0f, 1f, 0f, 0.7f), 64);

        cube1 = new GameObject(GetPath.GetCorrectPath("Assets/objects/cube.obj")); 
        cube1.Position = new Vector3(0, 0, -40);
        cube1.Scale = new Vector3(4f);
        cube1.Color = Colors.CornflowerBlue;
        _objects.Add(cube1);

        cube2 = new GameObject(GetPath.GetCorrectPath("Assets/objects/cube.obj")); 
        cube2.Position = new Vector3(0, 8, -40);
        cube2.Scale = new Vector3(4f);
        cube2.Color = Colors.Silver;
        _objects.Add(cube2);

        teapot1 = new GameObject(GetPath.GetCorrectPath("Assets/objects/teapot.obj")); 
        teapot1.Position = new Vector3(-15, 0, -40);
        teapot1.Scale = new Vector3(0.2f);
        teapot1.Color = Colors.Green;
        _objects.Add(teapot1);
    
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