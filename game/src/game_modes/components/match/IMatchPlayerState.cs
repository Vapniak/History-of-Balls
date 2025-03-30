namespace HOB;

using GameplayFramework;
using GameplayTags;
using HOB.GameEntity;

public interface IMatchPlayerState : IPlayerState, ITurnAware {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }

  public EntityData? GetEntity(Tag tag);
}
