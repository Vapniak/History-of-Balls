namespace HOB;


using GameplayFramework;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }

  public Array<ProductionConfig> ProducedEntities { get; set; }
  public Array<EntityData> Entities { get; set; }

  public HOBPlayerState(PlayerAttributeSet playerAttributeSet, Array<ProductionConfig> producedEntities, Array<EntityData> entities) : base() {
    ProducedEntities = producedEntities;
    Entities = entities;

    AbilitySystem = new();

    TreeEntered += () => {
      AddChild(AbilitySystem);
      AbilitySystem.Owner = this;

      AbilitySystem.AttributeSystem.AddAttributeSet(playerAttributeSet);
    };
  }

  public bool IsCurrentTurn() => GetController<IMatchController>().IsCurrentTurn();
}
