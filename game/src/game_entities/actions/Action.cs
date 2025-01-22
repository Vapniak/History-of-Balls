namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public abstract partial class Action : Node {
  [Signal] public delegate void FinishedEventHandler();

  public abstract void Start();
  public void Finish() {
    EmitSignal(SignalName.Finished);
  }
}
