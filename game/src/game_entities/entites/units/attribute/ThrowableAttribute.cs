namespace HOB;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class ThrowableAttribute : UnitAttribute {
  private Vector3 _gravity = new(0, -20f, 0);

  [ExportGroup("Throw")]
  [Export] private bool OrientTowardVelocity { get; set; } = true;
  [Export] private float ThrowHeight { get; set; } = 5f;
  [Export] private float RotationSmoothness { get; set; } = 5f;

  private Vector3 _launchVelocity;
  private Vector3 _startPosition;
  private Vector3 _initialOffset;
  private Quaternion _targetRotation;
  private Quaternion _initialRotation;

  public override void _Ready() {
    _initialOffset = Position;
    _initialRotation = Quaternion;
  }

  public override async Task DoAction(Vector3 toPosition) {
    await StartThrow(1);
  }

  public override async Task Reset() {
    TopLevel = false;

    Position = _initialOffset;
    Quaternion = _initialRotation;

    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.One, 0.1f);

    await ToSignal(tween, Tween.SignalName.Finished);
  }

  private async Task StartThrow(float duration) {
    // var startQuat = Basis.GetRotationQuaternion().Normalized();
    // TopLevel = true;

    // // Calculate direction with fallback
    // var direction = _launchVelocity + _gravity;
    // if (direction.LengthSquared() < 0.0001f) {
    //   direction = Vector3.Forward;
    // }

    // var throwDirection = direction.Normalized();
    // var upVector = Vector3.Up;

    // var targetBasis = Basis.LookingAt(throwDirection, upVector);
    // var targetQuat = targetBasis.GetRotationQuaternion().Normalized();


    // var windUpTween = CreateTween();
    // windUpTween.TweenMethod(Callable.From<float>(t => {
    //   if (startQuat != targetQuat) {
    //     var newQuat = startQuat.Normalized().Slerp(targetQuat.Normalized(), t).Normalized();
    //     Quaternion = newQuat;
    //   }
    // }), 0f, 1f, 0.3f);

    // windUpTween.Parallel()
    //     .TweenProperty(this, "position", new Vector3(0, -0.2f, -0.4f), 0.3f)
    //     .AsRelative()
    //     .SetEase(Tween.EaseType.Out);

    // await ToSignal(windUpTween, Tween.SignalName.Finished);

    // var throwTween = CreateTween();
    // throwTween.TweenMethod(Callable.From<float>(UpdateTrajectory), 0f, duration, duration)
    //     .SetEase(Tween.EaseType.Out);

    // await ToSignal(throwTween, Tween.SignalName.Finished);

    var fadeTween = CreateTween();
    fadeTween.TweenProperty(this, "scale", Vector3.Zero, 0.1f);
    await ToSignal(fadeTween, Tween.SignalName.Finished);
  }
}