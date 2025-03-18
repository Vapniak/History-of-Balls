namespace GameplayAbilitySystem;

using Godot;

public partial class GameplayAbilityInstance : Node {
  public GameplayAbility Ability { get; private set; }
  public int Level { get; private set; }
  public bool IsActive { get; private set; }

  private GameplayEventData CurrentData { get; set; }

  public GameplayAbilityInstance(GameplayAbility ability, int level) {
    Ability = ability;
    Level = level;
  }
}
