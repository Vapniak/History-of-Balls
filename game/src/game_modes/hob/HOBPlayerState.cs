namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }

  public HOBPlayerState(PlayerAttributeSet playerAttributeSet) : base() {
    AbilitySystem = new();
    AbilitySystem.AddAttributeSet(playerAttributeSet);
    AddChild(AbilitySystem);
    AbilitySystem.Owner = this;
  }

  public bool IsCurrentTurn() => GetController<IMatchController>().IsCurrentTurn();
}
