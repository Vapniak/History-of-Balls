namespace Tooltip;

using Godot;
using System;

public partial class TooltipManager : Node {
  public static TooltipManager Instance { get; private set; }

  [Export] public PackedScene TooltipScene { get; private set; }
  [Export] private Timer HideTimer { get; set; }
  [Export] private Timer ShowTimer { get; set; }

  private Tooltip Tooltip { get; set; }

  private bool _pendingHide;

  public override void _EnterTree() {
    base._EnterTree();

    ProcessMode = ProcessModeEnum.Always;

    if (Instance != null) {
      QueueFree();
    }

    Instance = this;
    Tooltip = TooltipScene.Instantiate<Tooltip>();
    AddChild(Tooltip);

    HideTooltip();
    HideTimer.Timeout += OnHideTimeout;
    ShowTimer.Timeout += Tooltip.ShowTooltip;
  }

  public override void _ExitTree() {
    if (Instance == this) {
      Instance = null;
    }
  }

  public void ShowTooltip(ITooltipTrigger trigger) {
    CancelHide();

    ShowTimer.Start();
    Tooltip.SetTooltip(trigger.Text, trigger.Position);
  }

  public void CancelHide() {
    HideTimer.Stop();
    _pendingHide = false;
  }

  public void RequestHide() {
    if (!Tooltip.IsMouseOver) {
      HideTimer.Start();
      _pendingHide = true;
    }
  }


  private void HideTooltip() {
    Tooltip.HideTooltip();
    ShowTimer.Stop();
  }

  private void OnHideTimeout() {
    if (_pendingHide) {
      HideTooltip();
      _pendingHide = false;
    }
  }
}
