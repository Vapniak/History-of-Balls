namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using System.Linq;

[GlobalClass]
public abstract partial class MoveAbility : HOBAbility {
  public override GameplayAbility.Instance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : EntityInstance {
    public const float MOVE_ANIMATION_SPEED = .2f;

    public Instance(MoveAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is MoveTargetData moveTarget) {
        return base.CanActivateAbility(eventData) && moveTarget.Cell != OwnerEntity.Cell;
      }
      return base.CanActivateAbility(eventData);
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      base.ActivateAbility(eventData);
      AddBlockTurn();
    }

    public override void EndAbility(bool wasCanceled = false) {
      base.EndAbility(wasCanceled);

      if (!wasCanceled) {
        RemoveBlockTurn();
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
  }
}
