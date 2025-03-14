namespace Tooltip;
using Godot;
using System;

[GlobalClass]
public partial class TooltipTrigger : Node, ITooltipTrigger {
  [Export] public string Text { get; private set; }
  public Vector2 Position => GetViewport().GetMousePosition();

  public TooltipTrigger(string text) {
    Text = text;
  }
  public override void _Ready() {
    base._Ready();

    GetParent<Control>().MouseEntered += () => TooltipManager.Instance.ShowTooltip(this);
    GetParent<Control>().MouseExited += TooltipManager.Instance.RequestHide;
  }
}
