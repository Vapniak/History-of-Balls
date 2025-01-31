namespace HOB.GameEntity;

using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CommandTrait : Trait {
  [Signal] public delegate void CommandSelectedEventHandler(Command command);

  // AAAAHHHH, the commands were shared between units and I spend 3 hours figuring out why all commands get changed when I change only one
  [Export] private Array<Command> Commands { get; set; }
  public override void _Ready() {
    base._Ready();

  }
  public void SelectCommand(Command command) => EmitSignal(SignalName.CommandSelected, command);
  public Command[] GetCommands() => Commands.ToArray();
}
