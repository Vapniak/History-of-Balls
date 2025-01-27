namespace HOB.GameEntity;

using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CommandTrait : Trait {
  [Export] private Array<Command> Commands { get; set; }

  public Command[] GetCommands() => Commands.ToArray();
}
