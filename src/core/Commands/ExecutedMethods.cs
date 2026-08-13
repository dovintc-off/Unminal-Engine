// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Commands.ExecutedMethods;

[SupportedOSPlatform("windows")]
public static class CalledMethods {
    public static bool Write(Dictionary<string, object> args) {
        Console.WriteLine(args["text"]);
        return true;
    }

    public static bool Fov_set(Dictionary<string, object> args) {
        if (Engine.Player.CameraObj == null) throw new Exception("[#red]Something went wrong, camera is null: see file ExecutedMethods.cs (line ~10)");
        
        float fov = Convert.ToSingle(args["value"]); 
        float min = Engine.Player.CameraObj.limitationFOV[0];
        float max = Engine.Player.CameraObj.limitationFOV[1];

        if (fov < min) {
            Console.WriteLine($"[#red] must be more then {min}");
            return false;
        }
        if (fov > max) { 
            Console.WriteLine($"[#red] must be smalest then {max}");
            return false;
        }

        if (Engine.Player.CameraObj != null)
        {
            Engine.Player.CameraObj.FOV = MathHelper.DegreesToRadians(fov);
        }

        return true;
    }

    public static bool Fov_get(Dictionary<string, object> args) {
        if (Engine.Player.CameraObj == null) throw new Exception("[#red]Something went wrong, camera is null: see file ExecutedMethods.cs (line ~30)");
        
        Console.WriteLine($"Player camera fov: {MathHelper.RadiansToDegrees(Engine.Player.CameraObj.FOV)}");
        return true;
    }

    public static bool ToggleLightDisplay(Dictionary<string, object> args)
    {
        Engine.ShowLight = !Engine.ShowLight;
        Console.WriteLine($"Light display: {(Engine.ShowLight ? "[#green]ON" : "[#red]OFF")}");
        return true;
    }

    public static bool SayHello(Dictionary<string, object> args){
        Log.Create(Log.LogType.INFO, "Hello!");
        return true;
    }

    public static bool ReloadScripts(Dictionary<string, object> args) {
        // if (Engine.GameInstance is Main.Main main) {
        //     main.ReloadScript();
        //     return true;
        // } else {
        //     Console.CreateLog(Console.LogType.ERROR, "Cannot access Main instance for reloading.");
        //     return false;
        // }
        return true;
    }
} 
