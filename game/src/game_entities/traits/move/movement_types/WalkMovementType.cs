namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class WalkMovementType : MovementType {
  private float _animSpeed = 0.2f;
  public override bool IsCellReachable(GameCell from, GameCell to) {
    var haveObstacle = false;

    foreach (var entity in MoveTrait.Entity.GameBoard.GetEntitiesOnCell(to)) {
      if (entity.TryGetTrait<ObstacleTrait>(out _)) {
        haveObstacle = true;
        break;
      }
    }

    return
      !haveObstacle &&
      to.GetSetting().MoveCost > 0 &&
      from.GetEdgeTypeTo(to) != GameCell.EdgeType.Cliff &&
      to.GetSetting().Elevation > 0;
  }

  public override async Task StartMoveOn(GameCell[] path) {
    for (var i = 1; i < path.Length; i++) {
      var from = path[i - 1];
      var to = path[i];

      await Walk(to);
    }

    MoveTrait.Entity.Cell = path.Last();

    EmitSignal(SignalName.MoveFinished);
  }

  private async Task Walk(GameCell to) {
    var startPosition = MoveTrait.Entity.GetPosition();
    var targetPosition = to.GetRealPosition();

    if (MoveTrait.Entity.GameBoard.GetEntitiesOnCell(to).Length > 0) {
      targetPosition.Y += 2;
    }

    var midpoint = (startPosition + targetPosition) / 2;
    midpoint.Y += 1.0f;

    var tween = MoveTrait.CreateTween();

    tween.TweenMethod(
        Callable.From<Vector3>(MoveTrait.Entity.SetPosition),
        startPosition,
        midpoint,
        _animSpeed
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

    MoveTrait.Entity.TurnAt(targetPosition, _animSpeed);


    tween.TweenMethod(
        Callable.From<Vector3>(MoveTrait.Entity.SetPosition),
        midpoint,
        targetPosition,
        _animSpeed
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

    await ToSignal(tween, Tween.SignalName.Finished);
  }
}
