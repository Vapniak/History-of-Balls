namespace HOB;

using GameplayAbilitySystem;
using Godot;

[GlobalClass]
public partial class ThrowAttackAbility : AttackAbility {
  [Export] public PackedScene? ThrowableObject { get; private set; }

  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : AttackAbility.Instance {
    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is AttackTargetData attackTargetData && CommitCooldown()) {
        var effect = (AbilityResource as AttackAbility)?.BlockMovementEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0);
          OwnerAbilitySystem.ApplyGameplayEffectToSelf(ge);
        }


        // TODO: throw
        //        var throwable = (AbilityResource as ThrowAttackAbility)?.ThrowableObject?.InstantiateOrNull<Node3D>();

        if (CurrentEventData?.TargetData is AttackTargetData d) {
          var damage = (AbilityResource as AttackAbility)?.DamageEffect;
          if (effect != null) {
            var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0);
            ge.Target = d.TargetAbilitySystem;
            d.TargetAbilitySystem.ApplyGameplayEffectToSelf(ge);
          }
        }

        EndAbility();
        return;
      }

      EndAbility();
    }
  }
}