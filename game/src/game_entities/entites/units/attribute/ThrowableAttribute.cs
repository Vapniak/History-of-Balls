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
    var height = ThrowHeight;
    var gravity = Mathf.Abs(_gravity.Y);

    var horizontalDir = new Vector3(toPosition.X - GlobalPosition.X, 0, toPosition.Z - GlobalPosition.Z);
    if (horizontalDir.LengthSquared() > 0) {
      horizontalDir = horizontalDir.Normalized();
    }

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
    Quaternion = _initialRotation;

    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.One, 0.1f);

    await ToSignal(tween, Tween.SignalName.Finished);
  }

  private async Task StartThrow(float duration) {
    var startQuat = Quaternion;
    TopLevel = true;

    var direction = _launchVelocity + (_gravity * 0.1f);
    if (direction.LengthSquared() < 0.0001f) {
      direction = Vector3.Forward;
    }
    var throwDirection = direction.Normalized();

    var windUpTween = CreateTween();


    var targetBasis = Basis.LookingAt(throwDirection, Vector3.Up);
    var targetQuat = targetBasis.GetRotationQuaternion().Normalized();

    windUpTween.TweenMethod(Callable.From<float>(t => {
      // FIXME: target quat is not normalized
      var newQuat = startQuat.Slerp(targetQuat, t).Normalized();
      Quaternion = newQuat;
    }), 0f, 1f, 0.3f);

    windUpTween.Parallel()
          .TweenProperty(this, "position", new Vector3(0, -.2f, -.4f), 0.3f)
          .AsRelative()
          .SetEase(Tween.EaseType.Out);

    await ToSignal(windUpTween, Tween.SignalName.Finished);

    var tween1 = CreateTween();
    tween1.TweenMethod(Callable.From<float>(UpdateTrajectory), 0f, duration, duration)
          .SetEase(Tween.EaseType.Out);

    await ToSignal(tween1, Tween.SignalName.Finished);

    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.Zero, .1f);
    await ToSignal(tween, Tween.SignalName.Finished);
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

      var targetQuat = targetBasis.GetRotationQuaternion().Normalized();

      Quaternion = Quaternion.Slerp(targetQuat, RotationSmoothness * (float)GetProcessDeltaTime()).Normalized();
    }
  }
}