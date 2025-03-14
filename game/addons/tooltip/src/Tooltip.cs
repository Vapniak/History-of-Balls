namespace Tooltip;

using Godot;
using System;

public partial class Tooltip : CanvasLayer {
  [Export] private Control TooltipContainer { get; set; }
  [Export] private RichTextLabel Label { get; set; }

  public bool IsMouseOver { get; private set; }

  private Tween Tween { get; set; }

  public override void _Ready() {
    Label.MouseEntered += OnTooltipMouseEntered;
    Label.MouseExited += OnTooltipMouseExited;

    TooltipContainer.Modulate = Colors.Transparent;
  }

  public void ShowTooltip() {
    Tween?.Kill();

    Tween = CreateTween();
    Tween.TweenProperty(TooltipContainer, "modulate", Colors.White, 0.2f);
  }

  public void HideTooltip() {
    Tween?.Kill();

    Tween = CreateTween();
    Tween.TweenProperty(TooltipContainer, "modulate", Colors.Transparent, 0.2f);
  }
  /// <summary>
  ///
  /// </summary>
  /// <param name="text">BBCode text</param>
  public void SetTooltip(string text, Vector2 position) {
    Label.Text = text;
    TooltipContainer.Position = GetAdjustedPosition(position);
  }

  public Vector2 GetAdjustedPosition(Vector2 targetPosition) {
    var viewport = GetViewport().GetVisibleRect().Size;
    var tooltipSize = TooltipContainer.GetSize();

    return new Vector2(
        Mathf.Clamp(targetPosition.X, 0, viewport.X - tooltipSize.X),
        Mathf.Clamp(targetPosition.Y + 20, 0, viewport.Y - tooltipSize.Y)
    );
  }


  private void OnTooltipMouseEntered() {
    IsMouseOver = true;
    TooltipContainer.SelfModulate = Colors.White;
    TooltipManager.Instance.CancelHide();
  }

  private void OnTooltipMouseExited() {
    IsMouseOver = false;
    TooltipContainer.SelfModulate = new(1, 1, 1, 0.8f);
    TooltipManager.Instance.RequestHide();
  }
}
