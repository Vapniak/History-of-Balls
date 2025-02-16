namespace HOB.GameEntity;

using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class WalkMovementType : MovementType {
  private float _animSpeed = 0.2f;
  public override bool IsCellReachable(GameCell from, GameCell to) {
    return
      MoveTrait.Entity.GameBoard.GetEntitiesOnCell(to).Length == 0 &&
      MoveTrait.Entity.GameBoard.GetSetting(to).MoveCost > 0 &&
      MoveTrait.Entity.GameBoard.GetEdgeType(from, to) != GameCell.EdgeType.Cliff &&
      MoveTrait.Entity.GameBoard.GetSetting(to).Elevation > 0;
  }

  public override void StartMoveOn(GameCell[] path) {
    Move(path);
  }


  private async Task Move(GameCell[] path) {
    for (var i = 1; i < path.Length; i++) {
      var from = path[i - 1];
      var to = path[i];

      await Walk(to);

      MoveTrait.Entity.Cell = to;
    }

    EmitSignal(SignalName.MoveFinished);
  }

  private async Task Walk(GameCell to) {
    var startPosition = MoveTrait.Entity.GetPosition();
    var targetPosition = MoveTrait.Entity.GameBoard.GetCellRealPosition(to);

    var targetRotation = Basis.LookingAt(startPosition.DirectionTo(targetPosition) * new Vector3(1, 0, 1)).GetRotationQuaternion();

    var midpoint = (startPosition + targetPosition) / 2;
    midpoint.Y += 1.0f;

    var tween = MoveTrait.CreateTween();

    tween.TweenMethod(
        Callable.From<Vector3>(MoveTrait.Entity.SetPosition),
        startPosition,
        midpoint,
        _animSpeed
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
    tween.Parallel().TweenProperty(MoveTrait.Entity.Body, "quaternion", targetRotation, _animSpeed).SetTrans(Tween.TransitionType.Cubic);


    tween.TweenMethod(
        Callable.From<Vector3>(MoveTrait.Entity.SetPosition),
        midpoint,
        targetPosition,
        _animSpeed
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

    await ToSignal(tween, Tween.SignalName.Finished);
  }
}
