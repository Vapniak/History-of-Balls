namespace HOB;

using GameplayAbilitySystem;
using Godot;
using HOB.GameEntity;
using System.Threading.Tasks;

[GlobalClass]
public partial class MoveAbility : HOBAbility {
  [Signal] public delegate void CellSelectedEventHandler(GameCell cell);
  public const float MOVE_ANIMATION_SPEED = 0.2f;

  public void SelectCellToMove(GameCell cell) {
    EmitSignal(SignalName.CellSelected, cell);
  }

  protected override void ActivateAbility(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo, GameplayEventData triggerEventData) {
    if (ownerInfo.OwnerNode is Entity entity) {
      _ = WaitForCellSelection(abilityInstance, ownerInfo);
    }
    else {
      EndAbility(abilityInstance, ownerInfo);
    }
  }

  public virtual GameCell[] GetReachableCells(GameplayAbilityOwnerInfo ownerInfo) {
    if (ownerInfo.OwnerNode is Entity entity) {
      if (ownerInfo.AbilitySystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
        return entity.Cell.ExpandSearch((uint)moveAttributeSet.MovePoints.CurrentValue, IsReachable);
      }
    }

    return null;
  }

  public virtual GameCell[] FindPathTo(GameplayAbilityOwnerInfo ownerInfo, GameCell cell) {
    if (ownerInfo.OwnerNode is Entity entity) {
      if (ownerInfo.AbilitySystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
        return entity.Cell.FindPathTo(cell, (uint)moveAttributeSet.MovePoints.CurrentValue, IsReachable);
      }
    }

    return null;
  }
  protected virtual bool IsReachable(GameCell from, GameCell to) {
    return !to.GetSetting().IsWater;
  }

  private async Task WaitForCellSelection(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    var awaiter = ToSignal(this, SignalName.CellSelected);
    await awaiter;

    var path = FindPathTo(ownerInfo, awaiter.GetResult()[0].As<GameCell>());

    if (ownerInfo.OwnerNode is Entity entity) {
      foreach (var cell in path) {
        await Walk(entity, cell);
      }
    }

    EndAbility(abilityInstance, ownerInfo);
  }

  private async Task Walk(Entity entity, GameCell to) {
    var startPosition = entity.GetPosition();
    var targetPosition = to.GetRealPosition();

    var midpoint = (startPosition + targetPosition) / 2;
    midpoint.Y += 1.0f;

    var tween = entity.CreateTween();

    tween.TweenMethod(
        Callable.From<Vector3>(entity.SetPosition),
        startPosition,
        midpoint,
        MOVE_ANIMATION_SPEED
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

    entity.Cell = to;

    entity.TurnAt(targetPosition, MOVE_ANIMATION_SPEED);

    tween.TweenMethod(
        Callable.From<Vector3>(entity.SetPosition),
        midpoint,
        targetPosition,
        MOVE_ANIMATION_SPEED
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

    await ToSignal(tween, Tween.SignalName.Finished);
  }
}
