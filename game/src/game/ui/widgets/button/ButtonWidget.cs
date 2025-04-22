namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class ButtonWidget : BaseButtonWidget<ButtonWidget, Button> {
  [Export] public override Button Button { get; protected set; } = default!;

  public override void _Ready() {
  }

  public override void _ExitTree() {
  }
}
