namespace HOB;

using Godot;
using System;
using System.Linq;
using WidgetSystem;

[GlobalClass, Tool]
public partial class CampaignWidget : ButtonWidget, IWidgetFactory<CampaignWidget> {
  [Export] public Label CampaignNumberLabel { get; private set; } = default!;
  [Export] public Label CampaignNameLabel { get; private set; } = default!;
  [Export] public Control TraitsParent { get; private set; } = default!;
  [Export] public TextureRect LockIcon { get; private set; } = default!;
  [Export] public TextureRect Image { get; private set; } = default!;

  [Export] private Texture2D Locked { get; set; } = default!;
  [Export] private Texture2D Unlocked { get; set; } = default!;
  [Export] private Control HoveredControl { get; set; } = default!;
  private const float HoverHeight = -15f;
  private const float AnimationDuration = 0.3f;
  private const float ScaleFactor = 1.03f;

  private Vector2 _originalPosition;
  private Vector2 _originalScale;
  private Tween _currentTween;

  public override void _Ready() {
    _originalPosition = HoveredControl.Position;
    _originalScale = HoveredControl.Scale;

    Button.MouseEntered += OnHoverStart;
    Button.MouseExited += OnHoverEnd;
  }

  public static CampaignWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://v7gus1sjddy3").Instantiate<CampaignWidget>();
  }

  public void BindTo(Campaign campaign) {
    CampaignNumberLabel.Text = campaign.Number;
    CampaignNameLabel.Text = campaign.Name;

    foreach (var child in TraitsParent.GetChildren()) {
      child.Free();
    }

    foreach (var trait in campaign.Traits) {
      var widget = CampaignTraitWidget.CreateWidget();
      widget.BindTo(trait);
      TraitsParent.AddChild(widget);
      if (trait != campaign.Traits.Last()) {
        TraitsParent.AddChild(new VSeparator());
      }
    }

    LockIcon.Texture = campaign.Locked ? Locked : Unlocked;
    Button.Disabled = campaign.Locked;

    Image.Texture = campaign.Image;
  }

  private void OnHoverStart() {
    _currentTween?.Kill();

    _currentTween = CreateTween()
        .SetEase(Tween.EaseType.Out)
        .SetTrans(Tween.TransitionType.Back);
    _currentTween.SetParallel();
    _currentTween.TweenProperty(HoveredControl, "position:y", _originalPosition.Y + HoverHeight, AnimationDuration);
    //_currentTween.TweenProperty(this, "scale", _originalScale * ScaleFactor, AnimationDuration);

  }

  private void OnHoverEnd() {
    _currentTween?.Kill();
    _currentTween = CreateTween()
        .SetEase(Tween.EaseType.Out)
        .SetTrans(Tween.TransitionType.Sine);
    _currentTween.SetParallel();

    _currentTween.TweenProperty(HoveredControl, "position", _originalPosition, AnimationDuration * 0.8f);
    //_currentTween.TweenProperty(this, "scale", _originalScale, AnimationDuration * 0.8f);
  }
}
