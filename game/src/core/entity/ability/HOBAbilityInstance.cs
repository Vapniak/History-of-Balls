namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;

public abstract partial class HOBAbilityInstance : GameplayAbilityInstance {
  public new HOBAbilityResource AbilityResource { get; private set; }
  protected HOBAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
    AbilityResource = abilityResource;
  }

  protected void AddBlockTurn() {
    GameInstance.GetGameMode<HOBGameMode>().GetTurnManagment().AddBlockTurn();
  }

  protected void RemoveBlockTurn() {
    GameInstance.GetGameMode<HOBGameMode>().GetTurnManagment().RemoveBlockTurn();
  }
}
