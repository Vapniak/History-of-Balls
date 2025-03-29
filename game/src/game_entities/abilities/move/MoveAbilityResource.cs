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

  public partial class MoveAbilityInstance : HOBEntityAbilityInstance {
    public const float MOVE_ANIMATION_SPEED = 0.2f;

    public MoveAbilityInstance(MoveAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (!CommitCooldown()) {
        EndAbility(eventData);
        return;
      }

      if (eventData?.TargetData is MoveTargetData moveTargetData) {
        _ = WalkByPath(GetOwner<Entity>(), moveTargetData.Cell);
      }
    }

    public virtual GameCell[] GetReachableCells() {
      if (OwnerAbilitySystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
        var mp = OwnerAbilitySystem.GetAttributeCurrentValue(moveAttributeSet.MovePoints);
        return OwnerEntity.Cell.ExpandSearch((uint)mp.GetValueOrDefault(), IsReachable);
      }

      return [];
    }
    public virtual GameCell[] FindPathTo(GameCell cell) {
      if (OwnerAbilitySystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
        var mp = OwnerAbilitySystem.GetAttributeCurrentValue(moveAttributeSet.MovePoints);
        return OwnerEntity.Cell.FindPathTo(cell, (uint)mp.GetValueOrDefault(), IsReachable);
      }

      return [];
    }

    protected virtual bool IsReachable(GameCell from, GameCell to) {
      return
        !to.GetSetting().IsWater &&
        !OwnerEntity.EntityManagment.GetEntitiesOnCell(to).Any(e => e.AbilitySystem.OwnedTags.HasExactTag(TagManager.GetTag(HOBTags.EntityTypeUnit))) &&
        from.GetEdgeTypeTo(to) != GameCell.EdgeType.Cliff;
    }

    private async Task WalkByPath(Entity entity, GameCell to) {
      foreach (var cell in FindPathTo(to)) {
        await Walk(OwnerEntity, cell);
      }

      EndAbility(CurrentEventData);

      var strucure = OwnerEntity.EntityManagment.GetEntitiesOnCell(to).FirstOrDefault(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure)));

      if (strucure != null && OwnerEntity.TryGetOwner(out var owner) && owner != null) {
        OwnerAbilitySystem.SendGameplayEvent(TagManager.GetTag(HOBTags.EventEntityCapture), new() { Activator = owner, TargetData = new() { Target = strucure } });
      }
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
