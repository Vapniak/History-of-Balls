namespace Tooltip;


using Godot;
using System;

[GlobalClass]
public partial class TooltipTrigger3D : Area3D, ITooltipTrigger {
  [Export] public string Text { get; private set; }

  Vector2 ITooltipTrigger.Position => GetViewport().GetCamera3D().UnprojectPosition(GlobalPosition);

  public override void _Ready() {
    base._Ready();

    MouseEntered += OnMouseEntered;
    MouseExited += OnMouseExited;
  }

  private void OnMouseEntered() {
    TooltipManager.Instance.ShowTooltip(this);
  }

  private void OnMouseExited() {
    TooltipManager.Instance.RequestHide();
  }
}
