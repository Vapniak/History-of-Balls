using Godot;
using System;

[GlobalClass, Tool]
public partial class DotFiller : Control {
  [Export]
  public Color DotColor { get; set; } = new Color(1, 1, 1, 0.5f);

  [Export]
  public float DotSize { get; set; } = 1.5f;

  [Export]
  public float DotSpacing { get; set; } = 8f;

  public override void _Ready() {
    // Set size flags to expand horizontally
    SizeFlagsHorizontal = SizeFlags.ExpandFill;
    Connect("draw", new Callable(this, nameof(OnDraw)));
  }

  private void OnDraw() {
    var size = GetRect().Size;
    float y = size.Y / 2;

    // Calculate how many dots we can fit
    int dotCount = (int)Math.Floor(size.X / DotSpacing);

    for (int i = 0; i < dotCount; i++) {
      float x = i * DotSpacing;
      DrawCircle(new Vector2(x, y), DotSize, DotColor);
    }
  }

  public override void _Notification(int what) {
    if (what == NotificationResized) {
      QueueRedraw();
    }
  }
}