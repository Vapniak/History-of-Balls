namespace HOB;

using Godot;
using System;
using Godot;

public partial class FloatingText : Node3D {
  [Export] private Label3D Label { get; set; }

  private static readonly string _sceneUID = "uid://dswl3q7q6in2p";

  public static FloatingText Create(string text, Color color) {
    var floatingText = ResourceLoader.Load<PackedScene>(_sceneUID).Instantiate<FloatingText>();

    floatingText.Label.Text = text;
    floatingText.Label.Modulate = color;

    return floatingText;
  }

  public void Animate() {
    var tween = CreateTween();

    tween.SetParallel(true);
    tween.TweenProperty(this, "global_position", GlobalPosition + (Vector3.Up * 2), 1f);
    tween.TweenProperty(this, "scale", Vector3.One * 0.5f, 1.0f).SetEase(Tween.EaseType.Out);
    tween.TweenProperty(Label, "transparency", 1, 1f);
    tween.Finished += QueueFree;
  }
}
