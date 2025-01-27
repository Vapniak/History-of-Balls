namespace HOB;

using Godot;
using System;

public partial class StatPanel : Control {
  [Export] private Label NameLabel { get; set; }

  [Export] private Label MovePointsLabel { get; set; }

  public void SetNameLabel(string name) {
    NameLabel.Text = name;
  }
  public void SetMovePointsLabel(int movePoints) {
    MovePointsLabel.Text = movePoints.ToString();
  }
}
