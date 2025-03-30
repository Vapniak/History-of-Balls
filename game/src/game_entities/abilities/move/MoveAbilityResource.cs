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
    return new Instance(this, abilitySystem);
  }

  public partial class Instance : HOBEntityAbilityInstance {
    public const float MOVE_ANIMATION_SPEED = 0.2f;

    public Instance(MoveAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is MoveTargetData moveTargetData && CommitCooldown()) {
        _ = WalkByPath(moveTargetData.Cell);
      }
      else {
        EndAbility(eventData);
      }
    }

    public virtual GameCell[] GetReachableCells() {
      if (OwnerAbilitySystem.AttributeSystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
        var mp = OwnerAbilitySystem.AttributeSystem.GetAttributeCurrentValue(moveAttributeSet.MovePoints);
        return OwnerEntity.Cell.ExpandSearch((uint)mp.GetValueOrDefault(), IsReachable);
      }

      return [];
    }
    public virtual GameCell[] FindPathTo(GameCell cell) {
      if (OwnerAbilitySystem.AttributeSystem.TryGetAttributeSet<MoveAttributeSet>(out var moveAttributeSet)) {
        var mp = OwnerAbilitySystem.AttributeSystem.GetAttributeCurrentValue(moveAttributeSet.MovePoints);
        return OwnerEntity.Cell.FindPathTo(cell, (uint)mp.GetValueOrDefault(), IsReachable);
      }

      return [];
    }

    protected virtual bool IsReachable(GameCell from, GameCell to) {
      return
        !to.GetSetting().IsWater &&
        !OwnerEntity.EntityManagment.GetEntitiesOnCell(to).Any(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeUnit))) &&
        from.GetEdgeTypeTo(to) != GameCell.EdgeType.Cliff;
    }

    private async Task WalkByPath(GameCell to) {
      var path = FindPathTo(to);

      if (path.Length > 0) {
        foreach (var cell in path) {
          await Walk(cell);
        }

        var strucure = OwnerEntity.EntityManagment.GetEntitiesOnCell(path.Last()).FirstOrDefault(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure)));

        if (strucure != null && OwnerEntity.TryGetOwner(out var owner) && owner != null) {
          OwnerAbilitySystem.SendGameplayEvent(TagManager.GetTag(HOBTags.EventEntityCapture), new() { Activator = owner, TargetData = new() { Target = strucure } });
        }
      }

      EndAbility(CurrentEventData);
    }

    private async Task Walk(GameCell to) {
      var startPosition = OwnerEntity.GetPosition();
      var targetPosition = to.GetRealPosition();

      var midpoint = (startPosition + targetPosition) / 2;
      midpoint.Y += 1.0f;

      var tween = OwnerEntity.CreateTween();

      tween.TweenMethod(
          Callable.From<Vector3>(OwnerEntity.SetPosition),
          startPosition,
          midpoint,
          MOVE_ANIMATION_SPEED
      ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

      OwnerEntity.Cell = to;

      OwnerEntity.TurnAt(targetPosition, MOVE_ANIMATION_SPEED);


      tween.TweenMethod(
          Callable.From<Vector3>(OwnerEntity.SetPosition),
          midpoint,
          targetPosition,
          MOVE_ANIMATION_SPEED
      ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

      await ToSignal(tween, Tween.SignalName.Finished);
    }
  }
}
