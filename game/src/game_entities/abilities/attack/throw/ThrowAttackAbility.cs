namespace HOB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class ThrowAttackAbility : AttackAbility {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : AttackAbility.Instance {
    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
      OwnerAbilitySystem.OwnedTags.TagRemoved += OnTagRemoved;
    }
    public override void _ExitTree() {
      base._ExitTree();

      if (IsInstanceValid(OwnerAbilitySystem) && IsInstanceValid(OwnerAbilitySystem.OwnedTags)) {
        OwnerAbilitySystem.OwnedTags.TagRemoved -= OnTagRemoved;
      }
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is AttackTargetData attackTargetData && CommitCooldown()) {
        var effect = (AbilityResource as AttackAbility)?.BlockMovementEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0);
          OwnerAbilitySystem.ApplyGameplayEffectToSelf(ge);
        }

        _ = Attack(attackTargetData.TargetAbilitySystem.GetOwner<Entity>());
        return;
      }

      EndAbility();
    }


    public override (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities(GameCell? fromCell = null) {
      fromCell ??= OwnerEntity.Cell;

      (var attackableEntities, var cellsInR) = base.GetAttackableEntities(fromCell);

      attackableEntities = attackableEntities.Where(e => e.Cell.Coord.Distance(fromCell.Coord) > 1).ToArray();
      cellsInR = cellsInR.Where(c => c.Coord.Distance(fromCell.Coord) > 1).ToArray();

      return (attackableEntities, cellsInR);
    }

    private async Task Attack(Entity entity) {
      await OwnerEntity.TurnAt(entity.Cell.GetRealPosition(), 0.1f);

      UnitAttribute? unitAttribute = null;
      if (OwnerEntity.Body is UnitBody unit) {
        unitAttribute = unit.UnitAttribute;
        if (unitAttribute != null) {
          await unitAttribute.DoAction(entity.Cell.GetRealPosition());
        }
      }

      if (CurrentEventData?.TargetData is AttackTargetData d) {
        var effect = (AbilityResource as AttackAbility)?.DamageEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0, d.TargetAbilitySystem);
          d.TargetAbilitySystem.ApplyGameplayEffectToSelf(ge);
          ShowDamageNumber(entity.Cell.GetRealPosition());
        }
      }


      EndAbility();
    }

    // FIXME: null reference when died?
    private void OnTagRemoved(Tag tag) {
      if (IsInstanceValid(tag) && tag == TagManager.GetTag(HOBTags.CooldownAttack)) {
        if (IsInstanceValid(OwnerEntity.Body) && OwnerEntity.Body is UnitBody unit) {
          unit.UnitAttribute?.Reset();
        }
      }
    }
  }
}