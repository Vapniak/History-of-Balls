namespace GameplayAbilitySystem;

using Godot;

public partial class GameplayModifierEvaluatedData : RefCounted {
  public GameplayAttribute Attribute { get; private set; }
  public GameplayModifierEvaluatedData(GameplayAttribute attribute) {
    Attribute = attribute;
  }
}