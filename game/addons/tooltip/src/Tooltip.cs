namespace Tooltip;

using Godot;
using System.Threading.Tasks;

public partial class Tooltip : CanvasLayer {
  [Export] private Control TooltipContainer { get; set; }
  [Export] private RichTextLabel Label { get; set; }

  public bool IsMouseOver { get; private set; }
  public new bool IsVisible() => TooltipContainer.Modulate.A > 0;

  private Tween _currentTween;

  public override void _Ready() {
    Label.MouseEntered += OnTooltipMouseEntered;
    Label.MouseExited += OnTooltipMouseExited;
    TooltipContainer.Modulate = Colors.Transparent;
  }

  public async Task ShowTooltip() {
    Show();
    _currentTween?.Kill();

    _currentTween = CreateTween();
    _currentTween.TweenProperty(TooltipContainer, "modulate", Colors.White, 0.1f);

    await ToSignal(_currentTween, Tween.SignalName.Finished);

  }

  public async Task HideTooltip() {
    _currentTween?.Kill();

    _currentTween = CreateTween();
    _currentTween.TweenProperty(TooltipContainer, "modulate", Colors.Transparent, 0.1f);

    await ToSignal(_currentTween, Tween.SignalName.Finished);

    Hide();
  }

  public void SetTooltip(string text, Vector2 position) {
    Label.Text = text;
    TooltipContainer.Position = GetAdjustedPosition(position);
  }

  private Vector2 GetAdjustedPosition(Vector2 targetPosition) {
    var viewport = GetViewport().GetVisibleRect().Size;
    var tooltipSize = TooltipContainer.GetSize();

    return new Vector2(
        Mathf.Clamp(targetPosition.X + 10, 0, viewport.X - tooltipSize.X),
        Mathf.Clamp(targetPosition.Y + 10, 0, viewport.Y - tooltipSize.Y)
    );
  }

  private void OnTooltipMouseEntered() {
    IsMouseOver = true;
    TooltipManager.Instance.CancelHide();
    TooltipContainer.SelfModulate = Colors.White;
  }

  private void OnTooltipMouseExited() {
    IsMouseOver = false;
    TooltipContainer.SelfModulate = new Color(1, 1, 1, 0.8f);
    TooltipManager.Instance.RequestHide();
  }
}