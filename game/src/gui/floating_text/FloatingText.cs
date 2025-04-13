namespace HOB;

using Godot;
using System.Threading.Tasks;

public partial class FloatingText : Node3D {
  [Export] public RichTextLabel? Label { get; private set; }
  [Export] private Sprite3D? Sprite { get; set; }

  private static readonly string _sceneUID = "uid://dswl3q7q6in2p";

  public static FloatingText Create() {
    var floatingText = ResourceLoader.Load<PackedScene>(_sceneUID).Instantiate<FloatingText>();

    if (floatingText.Label != null) {
      floatingText.Label.Text = "";
      floatingText.Label?.AppendText("[center]");
    }

    if (floatingText.Sprite != null) {
      floatingText.Sprite.Transparency = 1;
    }

    return floatingText;
  }

  public async Task Animate() {
    var tween = CreateTween();

    tween.TweenProperty(Sprite, "transparency", 0f, 1f).SetEase(Tween.EaseType.In);
    tween.Parallel().TweenProperty(this, "global_position", GlobalPosition + (Vector3.Up * 3), 1f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.InOut);
    tween.TweenInterval(1);
    await ToSignal(tween, Tween.SignalName.Finished);

    var second = CreateTween();
    second.SetParallel();
    second.TweenProperty(Sprite, "transparency", 1f, .5f).SetEase(Tween.EaseType.In);
    second.TweenProperty(this, "global_position", GlobalPosition - (Vector3.Up * 3), 1f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.InOut);

    await ToSignal(second, Tween.SignalName.Finished);
    QueueFree();
  }
}
