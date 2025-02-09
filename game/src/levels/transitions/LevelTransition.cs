namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class LevelTransition : CanvasLayer {
  [Signal] public delegate void FinishedEventHandler();

  public override void _Ready() {
    Layer = 128;
    ProcessMode = ProcessModeEnum.Always;
  }
  public override void _Input(InputEvent @event) {
    GetViewport().SetInputAsHandled();
  }
  private void Finish() {
    EmitSignal(SignalName.Finished);
  }
}
