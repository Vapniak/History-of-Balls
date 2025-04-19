namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;


[GlobalClass]
public partial class InitializeStatsAbility : HOBAbility {
  [Export] public GameplayEffectResource? InitializeStatsEffects { get; private set; }
  public override GameplayAbility.Instance CreateInstance(GameplayAbilitySystem abilitySystem) {
    ActivateOnGranted = true;
    RemoveOnEnd = true;
    ShowInUI = false;

    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : HOBAbility.Instance {
    public Instance(HOBAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      return true;
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      base.ActivateAbility(eventData);
      if (AbilityResource is InitializeStatsAbility ability && ability.InitializeStatsEffects != null) {
        var ei = OwnerAbilitySystem.MakeOutgoingInstance(ability.InitializeStatsEffects, 0);
        OwnerAbilitySystem.ApplyGameplayEffectToSelf(ei);
        EndAbility();
      }
    }
  }
}
