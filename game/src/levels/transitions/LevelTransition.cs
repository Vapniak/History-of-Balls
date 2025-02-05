namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class LevelTransition : Node {
  [Signal] public delegate void FinishedEventHandler();

  private void Finish() {
    EmitSignal(SignalName.Finished);
  }
}
