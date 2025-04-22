namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

public interface IMatchPlayerState : IPlayerState, ITurnAware {
  public Country Country { get; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }
  public Array<ProductionConfig> ProducedEntities { get; set; }
  public Array<EntityData> Entities { get; set; }
  public Theme Theme { get; }
}
