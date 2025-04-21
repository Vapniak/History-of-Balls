namespace HOB;

using GameplayFramework;
using Godot.Collections;
using HOB.GameEntity;

public interface IMatchPlayerState : IPlayerState, ITurnAware {
  public Country Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }
  public Array<ProductionConfig> ProducedEntities { get; set; }
  public Array<EntityData> Entities { get; set; }
}
