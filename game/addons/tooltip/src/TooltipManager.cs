namespace Tooltip;

using Godot;
using HOB;
using System.Threading.Tasks;

public partial class TooltipManager : Node {
  public static TooltipManager? Instance { get; private set; }

  [Export] public PackedScene? TooltipScene { get; private set; }
  [Export] private Timer? HideTimer { get; set; }
  [Export] private Timer? ShowTimer { get; set; }

  private Tooltip? Tooltip { get; set; }
  private bool _pendingHide;
  private ITooltipTrigger? _pendingTooltipTrigger;

  public override void _EnterTree() {
    base._EnterTree();

    ProcessMode = ProcessModeEnum.Always;

    Instance = this;
    Tooltip = TooltipScene?.Instantiate<Tooltip>();
    AddChild(Tooltip);

    _ = HideTooltip();

    if (HideTimer != null) {
      HideTimer.Timeout += () => _ = OnHideTimeout();
    }

    if (ShowTimer != null) {
      ShowTimer.Timeout += () => _ = OnShowTimeout();
    }
  }

  public override void _ExitTree() {
    if (Instance == this) {
      Instance = null;
    }
  }

  public void ShowTooltip(ITooltipTrigger trigger) {
    if (Tooltip != null && Tooltip.IsVisible()) {
      _pendingTooltipTrigger = trigger;
      RequestHide();
      return;
    }

    CancelHide();
    ShowTimer?.Start();
    if (trigger != null && trigger.Text != null) {
      Tooltip?.SetTooltip(trigger.Text, trigger.Position);
    }
  }

  public void CancelHide() {
    HideTimer?.Stop();
    _pendingHide = false;
  }

  public void RequestHide() {
    if (Tooltip != null && !Tooltip.IsMouseOver) {
      HideTimer?.Start();
      _pendingHide = true;
    }
  }

  private async Task HideTooltip() {
    ShowTimer?.Stop();
    if (Tooltip != null) {
      await Tooltip.HideTooltip();
    }
  }

  private async Task OnHideTimeout() {
    if (_pendingHide) {
      _pendingHide = false;
      await HideTooltip();

      if (_pendingTooltipTrigger != null) {
        ShowTooltip(_pendingTooltipTrigger);
        _pendingTooltipTrigger = null;
      }
    }
  }

  private async Task OnShowTimeout() {
    // FIXME: when hovering over one control and then fast on another and then on nothing it shows the tooltip but it shouldnt
    if (GetViewport().GuiGetHoveredControl().GetChildByType<TooltipTrigger>() != null && Tooltip != null) {
      await Tooltip.ShowTooltip();
    }
    //await Tooltip.ShowTooltip();
  }
}