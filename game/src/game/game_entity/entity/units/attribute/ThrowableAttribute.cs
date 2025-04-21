namespace HOB;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class ThrowableAttribute : UnitAttribute {
  [ExportGroup("Throw")]
  [Export] private bool OrientTowardVelocity { get; set; } = true;
  [Export] private float RotationSmoothness { get; set; } = 5f;
  private Vector3 _gravity = new(0, -20f, 0);
  private float ThrowHeight { get; set; } = 1.5f;

  private Vector3 _launchVelocity;
  private Vector3 _startPosition;
  private Vector3 _initialOffset;
  private Vector3 _initialRotation;

  public override void _Ready() {
    _initialOffset = Position;
    _initialRotation = Rotation;
  }

  public override async Task DoAction(Vector3 toPosition) {
    await base.DoAction(toPosition);

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
    await base.Reset();

    TopLevel = false;
    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.One, 0.1f);

    Position = _initialOffset;
    Rotation = _initialRotation;

    Visible = true;
    await ToSignal(tween, Tween.SignalName.Finished);
  }

  private async Task StartThrow(float duration) {
    var startQuat = Basis.GetRotationQuaternion().Normalized();
    TopLevel = true;

    var direction = _launchVelocity + _gravity;
    if (direction.LengthSquared() < 0.0001f) {
      direction = Vector3.Forward;
    }

    var throwDirection = direction.Normalized();
    var upVector = Vector3.Up;

    var targetBasis = Basis.LookingAt(throwDirection, upVector);
    var targetQuat = targetBasis.GetRotationQuaternion().Normalized();


    var windUpTween = CreateTween();
    windUpTween.TweenMethod(Callable.From<float>(t => {
      if (startQuat.Normalized() != targetQuat.Normalized()) {
        var newQuat = startQuat.Normalized().Slerp(targetQuat.Normalized(), t).Normalized();
        Quaternion = newQuat;
      }
    }), 0f, 1f, 0.3f);

    windUpTween.Parallel()
        .TweenProperty(this, "position", new Vector3(0, -0.2f, -0.4f), 0.3f)
        .AsRelative()
        .SetEase(Tween.EaseType.Out);

    await ToSignal(windUpTween, Tween.SignalName.Finished);

    var throwTween = CreateTween();
    throwTween.TweenMethod(Callable.From<float>(UpdateTrajectory), 0f, duration, duration)
        .SetEase(Tween.EaseType.Out);

    await ToSignal(throwTween, Tween.SignalName.Finished);

    var fadeTween = CreateTween();
    fadeTween.TweenProperty(this, "scale", Vector3.One * Mathf.Epsilon, 0.1f);
    await ToSignal(fadeTween, Tween.SignalName.Finished);
    Visible = false;
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