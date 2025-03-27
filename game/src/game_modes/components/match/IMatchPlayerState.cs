namespace HOB;

using GameplayFramework;

public interface IMatchPlayerState : IPlayerState, ITurnAware {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }
}
