namespace HOB;

using GameplayAbilitySystem;

public abstract partial class HOBAbilityInstance : GameplayAbilityInstance {
  public new HOBAbilityResource AbilityResource { get; private set; }
  protected HOBAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
    AbilityResource = abilityResource;
  }
}