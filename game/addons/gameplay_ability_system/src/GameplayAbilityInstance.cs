namespace GameplayAbilitySystem;

using Godot;

public partial class GameplayAbilityInstance : Node {
  public GameplayAbility Ability { get; private set; }
  public int Level { get; set; }
  public bool IsActive { get; set; }
  public GameplayAbilityOwnerInfo CurrentOwnerInfo { get; set; }
  public GameplayEventData CurrentEventData { get; set; }

  public GameplayAbilityInstance(GameplayAbility ability, int level) {
    Ability = ability;
    Level = level;
  }
}
