namespace HOB;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class ThrowableAttribute : UnitAttribute {
  [Export] private Vector3 _gravity = new(0, -9.8f, 0);

  [ExportGroup("Throw")]
  [Export] private bool OrientTowardVelocity { get; set; } = true;
  [Export] private float ThrowHeight { get; set; } = 5f;
  [Export] private float RotationSmoothness { get; set; } = 5f;

  private Vector3 _launchVelocity;
  private Vector3 _startPosition;
  private Tween? _tween;
  private Vector3 _initialOffset;
  private Quaternion _targetRotation;
  private Basis _initialBasis;

  public override void _Ready() {
    _initialOffset = Position;
    _initialBasis = Basis;
  }

  public override async Task DoAction(Vector3 toPosition) {
    var height = ThrowHeight;
    var gravity = Mathf.Abs(_gravity.Y);

    var horizontalDir = new Vector3(toPosition.X - GlobalPosition.X, 0, toPosition.Z - GlobalPosition.Z).Normalized();
    var horizontalDistance = GlobalPosition.DistanceTo(new Vector3(toPosition.X, GlobalPosition.Y, toPosition.Z));

    var time = Mathf.Sqrt(2 * height / gravity) + Mathf.Sqrt(2 * (height + toPosition.Y - GlobalPosition.Y) / gravity);
    var launchVelocity = horizontalDir * (horizontalDistance / time);
    launchVelocity.Y = Mathf.Sqrt(2 * gravity * height);

    _launchVelocity = launchVelocity;
    _startPosition = GlobalPosition;
    await StartThrow(time);
  }

  public override async Task Reset() {
    TopLevel = false;

    Position = _initialOffset;
    Basis = _initialBasis;

    Visible = true;

    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.One, 0.1f);

    await ToSignal(tween, Tween.SignalName.Finished);
  }

  private async Task StartThrow(float duration) {
    TopLevel = true;

    var throwDirection = (_launchVelocity + (_gravity * 0.1f)).Normalized();

    var windUpTween = CreateTween();

    var startQuat = Basis.GetRotationQuaternion().Normalized();

    var targetBasis = Basis.LookingAt(throwDirection, Vector3.Up);
    var targetQuat = targetBasis.GetRotationQuaternion().Normalized();

    windUpTween.TweenMethod(Callable.From<float>(t => {
      var newQuat = startQuat.Slerp(targetQuat, EaseOutQuad(t)).Normalized();
      Basis = new Basis(newQuat);
    }), 0f, 1f, 0.3f);

    windUpTween.Parallel()
          .TweenProperty(this, "position", new Vector3(0, -.2f, -.4f), 0.3f)
          .AsRelative()
          .SetEase(Tween.EaseType.Out);

    await ToSignal(windUpTween, Tween.SignalName.Finished);

    _tween = CreateTween();
    _tween.TweenMethod(Callable.From<float>(UpdateTrajectory), 0f, duration, duration)
          .SetEase(Tween.EaseType.Out);

    await ToSignal(_tween, Tween.SignalName.Finished);

    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.Zero, .1f);
    await ToSignal(tween, Tween.SignalName.Finished);

    Visible = false;
  }

  private static float EaseOutQuad(float t) {
    return 1f - ((1f - t) * (1f - t));
  }

  private void UpdateTrajectory(float time) {
    var newPosition = _startPosition +
                     (_launchVelocity * time) +
                     (0.5f * _gravity * time * time);

    GlobalPosition = newPosition;

    var currentVelocity = _launchVelocity + (_gravity * time);

    if (OrientTowardVelocity && currentVelocity.Length() > 0.1f) {
      var velocityDir = currentVelocity.Normalized();
      var targetBasis = Basis.LookingAt(velocityDir, Vector3.Up);

      var currentQuat = Basis.GetRotationQuaternion().Normalized();
      var targetQuat = targetBasis.GetRotationQuaternion().Normalized();

      var newQuat = currentQuat.Slerp(targetQuat, RotationSmoothness * (float)GetProcessDeltaTime());
      Basis = new Basis(newQuat.Normalized());
    }
  }
}
