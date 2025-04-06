namespace GameplayAbilitySystem;

using Godot;

public partial class GameplayEffectModData : RefCounted {
  public GameplayEffectInstance EffectInstance { get; private set; }
  public GameplayModifierEvaluatedData EvaluatedData { get; private set; }
  public GameplayAbilitySystem Target { get; private set; }
  public GameplayEffectModData(GameplayEffectInstance effectInstance, GameplayModifierEvaluatedData evaluatedData, GameplayAbilitySystem target) {
    EffectInstance = effectInstance;
    EvaluatedData = evaluatedData;
    Target = target;
  }
}