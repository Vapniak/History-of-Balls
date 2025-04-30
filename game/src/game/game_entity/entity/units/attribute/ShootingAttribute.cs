namespace HOB;

using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class ShootingAttribute : UnitAttribute {
  [Export] private Node3D ShootPoint { get; set; } = default!;
  [Export] private PackedScene Projectile { get; set; } = default!;
  [Export] private float ProjectileSpeed { get; set; } = 50f;
  [Export] private float KnockbackDistance { get; set; } = 0.5f;
  [Export] private float KnockbackDuration { get; set; } = 0.2f;
  [Export] private float ReturnDuration { get; set; } = 0.3f;

  private Vector3 _initialPosition;
  private Vector3 _initialRotation;

  public override void _Ready() {
    _initialPosition = Position;
    _initialRotation = Rotation;
  }

  public override async Task DoAction(Vector3 toPosition) {
    await base.DoAction(toPosition);

    var shootDirection = (toPosition - GlobalPosition).Normalized();
    // DebugDraw3D.DrawLine(ShootPoint.GlobalPosition, ShootPoint.GlobalPosition + shootDirection * 5, Colors.Red, 2.0f);
    // 1. Rotate to face target (fixed calculation)
    //    await RotateToFaceTarget(shootDirection);

    await FireProjectile(toPosition);

    // _ = ApplyKnockback(-shootDirection);

    // _ = ReturnToOriginalPosition();
  }

  // private async Task RotateToFaceTarget(Vector3 direction) {
  //   var targetYRotation = Mathf.Atan2(direction.X, direction.Z);

  //   var rotationTween = CreateTween();
  //   rotationTween.TweenProperty(this, "global_rotation:y", targetYRotation, 0.2f)
  //               .SetEase(Tween.EaseType.Out);

  //   await ToSignal(rotationTween, Tween.SignalName.Finished);
  // }

  private async Task FireProjectile(Vector3 targetPosition) {
    if (Projectile == null || ShootPoint == null)
      return;

    var projectile = Projectile.Instantiate<Node3D>();
    GetTree().Root.AddChild(projectile);

    // Set initial position and rotation
    projectile.GlobalPosition = ShootPoint.GlobalPosition;
    projectile.LookAt(targetPosition);

    float distance = ShootPoint.GlobalPosition.DistanceTo(targetPosition);
    float flightTime = distance / ProjectileSpeed;

    var projectileTween = CreateTween();
    projectileTween.TweenProperty(projectile, "global_position", targetPosition, flightTime)
                  .SetEase(Tween.EaseType.Out);

    await ToSignal(projectileTween, Tween.SignalName.Finished);
    projectile.QueueFree();
  }

  // private async Task ApplyKnockback(Vector3 direction) {
  //   var knockbackPosition = _initialPosition + (direction * KnockbackDistance);

  //   var knockbackTween = CreateTween();
  //   knockbackTween.TweenProperty(this, "position", knockbackPosition, KnockbackDuration)
  //                .SetEase(Tween.EaseType.Out);

  //   await ToSignal(knockbackTween, Tween.SignalName.Finished);
  // }

  // private async Task ReturnToOriginalPosition() {
  //   var returnTween = CreateTween();
  //   returnTween.SetParallel();
  //   returnTween.TweenProperty(this, "position", _initialPosition, ReturnDuration)
  //             .SetEase(Tween.EaseType.Out);
  //   returnTween.TweenProperty(this, "rotation", _initialRotation, ReturnDuration)
  //             .SetEase(Tween.EaseType.Out);

  //   await ToSignal(returnTween, Tween.SignalName.Finished);
  // }

  public override async Task Reset() {
    await base.Reset();
    Position = _initialPosition;
    Rotation = _initialRotation;
  }
}