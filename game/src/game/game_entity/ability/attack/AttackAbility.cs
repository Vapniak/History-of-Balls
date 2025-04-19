namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[GlobalClass]
public abstract partial class AttackAbility : HOBAbility {
  [Export] public GameplayEffectResource? DamageEffect { get; private set; }
  [Export] public GameplayEffectResource? BlockMovementEffect { get; private set; }

  public partial class Instance : EntityInstance {
    public Instance(HOBAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

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

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is AttackTargetData data) {
        return GetAttackableEntities().entities.Contains(data.TargetAbilitySystem.GetOwner<Entity>()) && base.CanActivateAbility(eventData);
      }

      return base.CanActivateAbility(eventData);
    }
    public virtual (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities(GameCell? fromCell = null) {
      var attackableEntities = new List<Entity>();
      var cellsInR = new List<GameCell>();

      fromCell ??= OwnerEntity.Cell;

      var cells = fromCell.GetCellsInRange(GetRange());
      foreach (var cell in cells) {
        if (cell == fromCell || !IsCellVisible(fromCell, cell)) {
          continue;
        }

        cellsInR.Add(cell);
        var entities = OwnerEntity.EntityManagment.GetEntitiesOnCell(cell);
        attackableEntities.AddRange(entities.Where(CanBeAttacked));
      }

      return (attackableEntities.ToArray(), cellsInR.ToArray());
    }

    public virtual bool IsCellVisible(GameCell from, GameCell to) => true;

    public virtual bool CanBeAttacked(Entity entity) {
      return entity.TryGetOwner(out var enemyOwner) && OwnerEntity.TryGetOwner(out var owner) && enemyOwner != owner && entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeUnit));
    }

    public virtual uint GetRange() {
      if (OwnerEntity.AbilitySystem.AttributeSystem.TryGetAttributeSet<AttackAttributeSet>(out var attributeSet)) {
        return (uint)OwnerEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(attributeSet.Range).GetValueOrDefault();
      }
      else {
        Debug.Assert(false, "Entity doesnt have attack attribute set");
        return 0;
      }
    }
  }
}
