namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public ResourceType PrimaryResourceType { get; set; }
  public ResourceType SecondaryResourceType { get; set; }
}
