using GameplayFramework;
using Godot;
using System;

public partial class MiniMap : MarginContainer {
  [Export]
  private Camera3D Camera { get; set; }

  public override void _Process(double delta) {
    Camera.GlobalPosition = GetViewport().GetCamera3D().GetParent<Node3D>().GlobalPosition + (Vector3.Up * 1000);
  }
}
