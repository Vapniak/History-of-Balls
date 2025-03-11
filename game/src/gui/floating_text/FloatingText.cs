namespace HOB;

using Godot;
using System;
using Godot;
using System.Threading.Tasks;
using System.Collections;

public partial class FloatingText : Node3D {
  [Export] private Label3D Label { get; set; }

  private static readonly string _sceneUID = "uid://dswl3q7q6in2p";

  public static FloatingText Create(string text, Color color) {
    var floatingText = ResourceLoader.Load<PackedScene>(_sceneUID).Instantiate<FloatingText>();

    floatingText.Label.Text = text;
    floatingText.Label.Modulate = color;

    return floatingText;
  }

  public async Task Animate() {
    var tween = CreateTween();

    tween.TweenProperty(this, "global_position", GlobalPosition + (Vector3.Up * 2), 1.5f);

    var timer = new Timer();
    AddChild(timer);
    timer.Start(.5f);
    await ToSignal(timer, Timer.SignalName.Timeout);
    timer.QueueFree();

    var tween2 = CreateTween();
    tween2.SetParallel(true);
    tween2.Finished += QueueFree;
    tween2.TweenProperty(this, "scale", Vector3.One * 0.5f, 1.0f).SetEase(Tween.EaseType.Out);
    tween2.TweenProperty(Label, "transparency", 1, 1f);
  }
}
