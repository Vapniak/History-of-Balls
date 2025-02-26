namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class WalkMoveTrait : MoveTrait {
  private float _animSpeed = 0.2f;
  public override bool IsCellReachable(GameCell from, GameCell to) {
    var haveObstacle = false;

    foreach (var entity in Entity.GameBoard.GetEntitiesOnCell(to)) {
      if (entity.TryGetTrait<ObstacleTrait>(out _)) {
        haveObstacle = true;
        break;
      }
    }

    return
      !haveObstacle &&
      from.GetEdgeTypeTo(to) != GameCell.EdgeType.Cliff &&
      !to.GetSetting().IsWater;
  }

  public override async Task Move(GameCell targetCell) {
    var path = FindPath(targetCell);


    for (var i = 0; i < path.Length; i++) {
      var to = path[i];

      Entity.Cell = to;

      await Walk(to);
    }

    await base.Move(targetCell);
  }

  private async Task Walk(GameCell to) {
    var startPosition = Entity.GetPosition();
    var targetPosition = to.GetRealPosition();

    var midpoint = (startPosition + targetPosition) / 2;
    midpoint.Y += 1.0f;

    var tween = CreateTween();

    tween.TweenMethod(
        Callable.From<Vector3>(Entity.SetPosition),
        startPosition,
        midpoint,
        _animSpeed
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

    Entity.TurnAt(targetPosition, _animSpeed);


    tween.TweenMethod(
        Callable.From<Vector3>(Entity.SetPosition),
        midpoint,
        targetPosition,
        _animSpeed
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

    await ToSignal(tween, Tween.SignalName.Finished);
  }
}
