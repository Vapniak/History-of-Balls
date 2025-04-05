namespace HOB;


using GameplayFramework;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }

  public Array<ProductionConfig> ProducedEntities { get; set; }

  public HOBPlayerState(PlayerAttributeSet playerAttributeSet, Array<ProductionConfig> producedEntities) : base() {
    ProducedEntities = producedEntities;

    AbilitySystem = new();

    TreeEntered += () => {
      AddChild(AbilitySystem);
      AbilitySystem.Owner = this;

      AbilitySystem.AttributeSystem.AddAttributeSet(playerAttributeSet);
    };
  }

  public bool IsCurrentTurn() => GetController<IMatchController>().IsCurrentTurn();
}
