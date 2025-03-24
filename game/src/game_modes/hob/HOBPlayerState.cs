namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public Country? Country { get; set; }
  [Notify] public int PrimaryResourceValue { get; set; }
  [Notify] public int SecondaryResourceValue { get; set; }
}
