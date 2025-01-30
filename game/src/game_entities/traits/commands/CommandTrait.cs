namespace HOB.GameEntity;

using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CommandTrait : Trait {
  [Signal] public delegate void CommandSelectedEventHandler(Command command);

  [Export] private Array<Command> Commands { get; set; }
  public override void _Ready() {
    base._Ready();

    foreach (var command in GetCommands()) {
      command.Entity = GetEntity();
    }
  }
  public void SelectCommand(Command command) => EmitSignal(SignalName.CommandSelected, command);
  public Command[] GetCommands() => Commands.ToArray();
}
