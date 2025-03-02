namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class AttackTrait : Trait {
  [Signal] public delegate void AttackFinishedEventHandler();

  private List<Entity> AttackableEntities { get; set; }

  public async Task Attack(Entity entity) {
    await Entity.TurnAt(entity.Cell.GetRealPosition(), 0.1f);

    var firstTween = CreateTween();
    firstTween.TweenMethod(Callable.From<Vector3>(Entity.SetPosition), Entity.GetPosition(), Entity.GetPosition() + entity.GetPosition().Normalized(), .2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

    await ToSignal(firstTween, Tween.SignalName.Finished);

    if (entity.TryGetTrait<HealthTrait>(out var healthTrait)) {
      healthTrait.Damage(GetStat<AttackStats>().Damage);
    }

    var secondTween = CreateTween();
    secondTween.TweenMethod(Callable.From<Vector3>(Entity.SetPosition), Entity.GetPosition(), Entity.Cell.GetRealPosition(), .2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

    await ToSignal(secondTween, Tween.SignalName.Finished);

    EmitSignal(SignalName.AttackFinished);
  }

  public (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities() {
    AttackableEntities = new List<Entity>();
    var cellsInR = new List<GameCell>();

    var cells = Entity.Cell.GetCellsInRange(GetStat<AttackStats>().Range);
    foreach (var cell in cells) {
      if (cell == Entity.Cell) {
        continue;
      }

      cellsInR.Add(cell);
      var entities = Entity.GameBoard.GetEntitiesOnCell(cell);
      AttackableEntities.AddRange(entities.Where(CanBeAttacked));
    }

    return (AttackableEntities.ToArray(), cellsInR.ToArray());
  }

  public bool CanBeAttacked(Entity entity) {
    return entity.TryGetOwner(out var enemyOwner) && Entity.TryGetOwner(out var owner) && enemyOwner != owner && entity.TryGetTrait<HealthTrait>(out _);
  }
}
