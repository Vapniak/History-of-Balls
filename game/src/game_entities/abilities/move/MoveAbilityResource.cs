namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class MoveAbilityResource : HOBAbilityResource {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new MoveAbilityInstance(this, abilitySystem);
  }

  public partial class MoveAbilityInstance : HOBAbilityInstance {
    public const float MOVE_ANIMATION_SPEED = 0.2f;

    public MoveAbilityInstance(MoveAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override async Task ActivateAbility(GameplayEventData? eventData) {
      if (!CommitCooldown()) {
        await EndAbility(eventData);
        return;
      }

      if (eventData?.TargetData is MoveTargetData data) {
        foreach (var cell in FindPathTo(data.Cell)) {
          await Walk(OwnerAbilitySystem.GetOwner<Entity>(), cell);
        }
      }

      await EndAbility(eventData);
    }

    public virtual GameCell[] GetReachableCells() {
      if (OwnerAbilitySystem.GetOwner() is Entity entity) {
        if (OwnerAbilitySystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
          var mp = OwnerAbilitySystem.GetAttributeCurrentValue(moveAttributeSet.MovePoints);
          return entity.Cell.ExpandSearch((uint)mp.GetValueOrDefault(), IsReachable);
        }
      }

      return [];
    }
    public virtual GameCell[] FindPathTo(GameCell cell) {
      if (OwnerAbilitySystem.GetOwner() is Entity entity) {
        if (OwnerAbilitySystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
          var mp = OwnerAbilitySystem.GetAttributeCurrentValue(moveAttributeSet.MovePoints);
          return entity.Cell.FindPathTo(cell, (uint)mp.GetValueOrDefault(), IsReachable);
        }
      }

      return [];
    }

    protected virtual bool IsReachable(GameCell from, GameCell to) {
      return
        !to.GetSetting().IsWater &&
        !OwnerAbilitySystem.GetOwner<Entity>().EntityManagment.GetEntitiesOnCell(to).Any(e => e.AbilitySystem.OwnedTags.HasExactTag(TagManager.GetTag(HOBTags.EntityTypeUnit))) &&
        from.GetEdgeTypeTo(to) != GameCell.EdgeType.Cliff;
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
}
