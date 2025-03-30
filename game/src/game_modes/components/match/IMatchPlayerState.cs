namespace HOB;

using GameplayFramework;
using Godot.Collections;

public interface IMatchPlayerState : IPlayerState, ITurnAware {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }
  public Array<ProductionConfig> ProducedEntities { get; set; }
}
