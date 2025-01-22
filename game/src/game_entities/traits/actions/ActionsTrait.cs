namespace HOB.GameEntity;

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ActionsTrait : Trait {
  [Export] public Array<Action> Actions { get; private set; }
}
