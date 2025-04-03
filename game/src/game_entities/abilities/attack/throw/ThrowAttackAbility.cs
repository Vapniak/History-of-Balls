namespace HOB;

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

    protected override void Dispose(bool disposing) {
      base.Dispose(disposing);

      OwnerAbilitySystem.OwnedTags.TagRemoved -= OnTagRemoved;
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

    private async Task Attack(Entity entity) {
      await OwnerEntity.TurnAt(entity.Cell.GetRealPosition(), 0.1f);

      UnitAttribute? unitAttribute = null;
      if (OwnerEntity.Body is UnitBody unit) {
        unitAttribute = unit.UnitAttribute;
        if (unitAttribute != null) {
          await unitAttribute.ThrowToPosition(entity.Cell.GetRealPosition());
        }
      }

      if (CurrentEventData?.TargetData is AttackTargetData d) {
        var effect = (AbilityResource as AttackAbility)?.DamageEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0);
          ge.Target = d.TargetAbilitySystem;
          d.TargetAbilitySystem.ApplyGameplayEffectToSelf(ge);
          ShowDamageNumber(entity.Cell.GetRealPosition());
        }
      }


      EndAbility();
    }

    private void OnTagRemoved(Tag tag) {
      if (tag == TagManager.GetTag(HOBTags.CooldownAttack)) {
        if (OwnerEntity.Body is UnitBody unit) {
          unit.UnitAttribute?.Reset();
        }
      }
    }
  }
}