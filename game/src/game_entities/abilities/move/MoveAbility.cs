namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public abstract partial class MoveAbility : HOBAbilityResource {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public partial class Instance : HOBEntityAbilityInstance {
    public const float MOVE_ANIMATION_SPEED = 0.2f;

    public Instance(MoveAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

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
