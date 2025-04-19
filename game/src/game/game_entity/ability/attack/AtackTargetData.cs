namespace HOB;

using GameplayAbilitySystem;

public class AttackTargetData : GameplayAbilityTargetData {
  public required HOBGameplayAbilitySystem TargetAbilitySystem { get; set; }
}