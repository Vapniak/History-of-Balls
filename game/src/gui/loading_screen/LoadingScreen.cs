using GameplayFramework;
using Godot;
using System;

public partial class LoadingScreen : CanvasLayer {
  [Export] private ProgressBar ProgressBar { get; set; }

  public void SetProgressBarValue(float value) {
    ProgressBar.Value = value;
  }
}
