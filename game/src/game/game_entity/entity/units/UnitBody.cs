namespace HOB;

using System.Threading.Tasks;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class UnitBody : EntityBody {
  [Export] public UnitAttribute? UnitAttribute { get; private set; }

  private Tween? IdleAnimTween { get; set; }

  public override void _Ready() {
    base._Ready();

    StartIdleAnim();
  }
  public void StartIdleAnim() {
    IdleAnimTween?.Kill();

    IdleAnimTween = CreateTween();
    IdleAnimTween.SetLoops();
    IdleAnimTween.SetTrans(Tween.TransitionType.Back);
    var strech = 1.05f;
    var squash = 0.95f;
    IdleAnimTween.TweenProperty(this, "scale", new Vector3(strech, squash, strech), 0.5f);
    IdleAnimTween.TweenProperty(this, "scale", new Vector3(squash, strech, squash), 0.5f);
  }

  public async Task StopIdleAnim() {
    if (IdleAnimTween == null) {
      return;
    }

    IdleAnimTween.Kill();

    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.One, 0.1f);
    await ToSignal(tween, Tween.SignalName.Finished);
  }
}
