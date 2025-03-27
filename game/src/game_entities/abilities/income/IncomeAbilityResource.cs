namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class IncomeAbilityResource : HOBAbilityResource {
  [Export] public GameplayEffectResource? IncomeEffect { get; private set; }
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new IncomeAbilityInstance(this, abilitySystem);
  }


  public partial class IncomeAbilityInstance : HOBEntityAbilityInstance {
    public IncomeAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (OwnerEntity.TryGetOwner(out var owner)) {
        var ei = OwnerAbilitySystem.MakeOutgoingInstance((AbilityResource as IncomeAbilityResource).IncomeEffect, 0);
        ei.Target = owner.GetPlayerState().AbilitySystem;
        owner.GetPlayerState().AbilitySystem.TryApplyGameplayEffectToSelf(ei);
        EndAbility(eventData);
      }
    }
  }
}
