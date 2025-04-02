namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public partial class InitializeStatsAbilityResource : HOBAbilityResource {
  [Export] public GameplayEffectResource? InitializeStatsEffects { get; private set; }
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    ActivateOnGranted = true;
    RemoveOnEnd = true;
    ShowInUI = false;

    return new Instance(this, abilitySystem);
  }

  public partial class Instance : HOBAbilityInstance {
    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      return true;
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (AbilityResource is InitializeStatsAbilityResource ability && ability.InitializeStatsEffects != null) {
        var ei = OwnerAbilitySystem.MakeOutgoingInstance(ability.InitializeStatsEffects, 0);
        ei.Target = OwnerAbilitySystem;
        OwnerAbilitySystem.ApplyGameplayEffectToSelf(ei);
        EndAbility();
      }
    }
  }
}
