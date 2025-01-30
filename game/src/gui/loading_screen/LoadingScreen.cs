using GameplayFramework;
using Godot;
using System;

public partial class LoadingScreen : CanvasLayer {
  [Export] private ProgressBar ProgressBar { get; set; }

  public override void _Process(double delta) {
    Game.GetWorld().GetLevelLoadStatus(out _, out var progress);

    ProgressBar.Value = progress;
  }
}
