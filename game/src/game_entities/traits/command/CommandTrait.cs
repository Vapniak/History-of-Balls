namespace HOB.GameEntity;

using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class CommandTrait : Trait {
  [Signal] public delegate void CommandSelectedEventHandler(Command command);

  // AAAAHHHH, the commands were shared between units and I spend 3 hours figuring out why all commands get changed when I change only one
  private List<Command> Commands { get; set; }
  public override void _EnterTree() {
    base._EnterTree();

    Commands = new();

    foreach (var child in GetChildren()) {
      if (child is Command command) {
        Commands.Add(command);
        command.CommandTrait = this;
      }
    }
  }
  public void SelectCommand(Command command) => EmitSignal(SignalName.CommandSelected, command);
  public Command[] GetCommands() => Commands.ToArray();
  public bool TryGetCommand<T>(out T command) where T : Command {
    command = Commands.OfType<T>().FirstOrDefault();
    if (command != null) {
      return true;
    }

    return false;
  }
}
