namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public partial class MoveAbilityResource : HOBAbilityResource {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new MoveAbilityInstance(this, abilitySystem);
  }

  public partial class MoveAbilityInstance : HOBAbilityInstance {
    public MoveAbilityInstance(MoveAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }
  }
}
