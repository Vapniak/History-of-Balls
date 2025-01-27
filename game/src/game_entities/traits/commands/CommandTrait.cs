namespace HOB.GameEntity;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CommandTrait : Trait {
  [Export] private Array<Command> Commands { get; set; }
}
