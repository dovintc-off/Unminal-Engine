namespace Unminal.Core.Commands.Structure;
public class Command {
    public string? Name {get; set;}
    public List<Command> Layer {get; set;} = new List<Command>(); 
    public string? ExecuteMethod {get; set;}
    public bool ExecutedLayer {get; set;}
    public Dictionary<string, string> ArgsExecuteMethod {get; set;} = new Dictionary<string, string>();
    public Dictionary<string, Dictionary<string, string>> AdditionalArgs {get; set;} = new Dictionary<string, Dictionary<string, string>>();
    public CastomArgs? castomArgs {get; set;}
    public Command? this[string subCommandName] { get {
            foreach (var cmd in Layer) if (cmd.Name != null && cmd.Name.Equals(subCommandName, System.StringComparison.OrdinalIgnoreCase)) return cmd;
            return null;
        }
    }
    public void error(string reason){} // logic for error metod
    public void range(int s, int e){} // logic for range metod
    public void get(string key){} // logic for get metod
}
public class CastomArgs { 
    // Creates a User
}