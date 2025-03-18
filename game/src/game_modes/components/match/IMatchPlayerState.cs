namespace HOB;

using GameplayFramework;
using HOB.GameEntity;

public interface IMatchPlayerState : IPlayerState {
  public ResourceType PrimaryResourceType { get; set; }
  public ResourceType SecondaryResourceType { get; set; }
}
