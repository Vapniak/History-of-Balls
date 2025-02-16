namespace HOB;

using GameplayFramework;

public interface IMatchPlayerState : IPlayerState {
  public ResourceType PrimaryResourceType { get; set; }
  public ResourceType SecondaryResourceType { get; set; }
}
