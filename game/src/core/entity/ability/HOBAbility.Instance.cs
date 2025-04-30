namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using Godot;

public partial class HOBAbility {
  public new partial class Instance : GameplayAbility.Instance {
    public new HOBAbility AbilityResource { get; private set; }
    public Instance(HOBAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
      AbilityResource = abilityResource;
    }

    protected void AddBlockTurn() {
      GameInstance.GetGameMode<HOBGameMode>().GetTurnManagment().AddBlockTurn();
    }

    protected void RemoveBlockTurn() {
      GameInstance.GetGameMode<HOBGameMode>().GetTurnManagment().RemoveBlockTurn();
    }
  }
}