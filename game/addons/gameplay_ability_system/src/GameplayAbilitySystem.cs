namespace GameplayAbilitySystem;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameplayAbilitySystem : Node {
  [Export] private Array<GameplayAttributeSet> AttributeSets { get; set; } = new();

  private Godot.Collections.Dictionary<GameplayAttribute, GameplayAttributeValue> AttributeValues { get; set; } = new();
  private List<GameplayEffectContainer> AppliedEffects { get; set; } = new();
  private List<GameplayAbilityInstance> GrantedAbilities { get; set; } = new();

  public void GrantAbility(GameplayAbilityInstance abilityInstance) {
    GrantedAbilities.Add(abilityInstance);
  }

  public IEnumerable<GameplayAbilityInstance> GetGrantedAbilities() => GrantedAbilities;

  public bool TryApplyGameplayEffectToSelf(GameplayEffectInstance geInstance) {
    if (geInstance == null) {
      return false;
    }

    switch (geInstance.GameplayEffect.EffectDefinition.DurationPolicy) {
      case DurationPolicy.Duration:
        break;
      case DurationPolicy.Infinite:
        break;
      case DurationPolicy.Instant:
        ApplyInstantGameplayEffect(geInstance);
        break;
      default:
        break;
    }

    return true;
  }

  public void AddAttributeSet(GameplayAttributeSet attributeSet) {
    if (attributeSet == null) {
      return;
    }

    AttributeSets.Add(attributeSet);

    foreach (var attribute in attributeSet.GetAttributes()) {
      AttributeValues.TryAdd(attribute, new());
    }
  }

  public void RemoveAttributeSet(GameplayAttributeSet attributeSet) {
    AttributeSets.Remove(attributeSet);
    foreach (var attribute in attributeSet.GetAttributes()) {
      AttributeValues.Remove(attribute);
    }
  }

  public Godot.Collections.Dictionary<GameplayAttribute, GameplayAttributeValue> GetAllAttributes() {
    return AttributeValues;
  }

  public bool TryGetAttributeSet<T>(out T attributeSet) where T : GameplayAttributeSet {
    attributeSet = AttributeSets.OfType<T>().FirstOrDefault();

    return attributeSet != null;
  }

  public bool TryGetAttributeCurrentValue(GameplayAttribute attribute, out float? currentValue) {
    if (TryGetAttributeValue(attribute, out var value)) {
      currentValue = value.CurrentValue;
      return true;
    }

    currentValue = null;
    return false;
  }

  public bool TryGetAttributeBuffedValue(GameplayAttribute attribute, ref float? buffedValue) {
    if (TryGetAttributeValue(attribute, out var value)) {
      buffedValue = value.BuffedValue;
      return true;
    }

    buffedValue = null;
    return false;
  }

  public void TickGameplayEffectsBy(float delta) {
    foreach (var effect in AppliedEffects) {
      var ge = effect.EffectInstance;

      if (ge.GameplayEffect.EffectDefinition.DurationPolicy == DurationPolicy.Instant) {
        continue;
      }

      ge.UpdateRemainingDurationBy(delta);

      ge.TickPeriodic(delta, out var executePeriodic);

      if (executePeriodic) {
        ApplyInstantGameplayEffect(ge);
      }
    }
  }

  private bool TryGetAttributeValue(GameplayAttribute attribute, out GameplayAttributeValue value) {
    return AttributeValues.TryGetValue(attribute, out value);
  }

  public GameplayEffectInstance MakeOutgoingInstance(GameplayEffectResource gameplayEffect, float level) {
    return GameplayEffectInstance.CreateNew(gameplayEffect, this, level);
  }

  private void ApplyInstantGameplayEffect(GameplayEffectInstance geInstance) {
    foreach (var modifier in geInstance.GameplayEffect.EffectDefinition.Modifiers) {
      var magnitue = modifier.ModifierMagnitude.CalculateMagnitude(geInstance);
      var attribute = modifier.Attribute;

      float? buffedValue = 0;
      if (TryGetAttributeBuffedValue(attribute, ref buffedValue)) {
        switch (modifier.ModifierType) {
          case AttributeModifierType.Add:
            buffedValue += magnitue;
            break;
          case AttributeModifierType.Multiply:
            buffedValue *= magnitue;
            break;
          case AttributeModifierType.Override:
            buffedValue = magnitue;
            break;
          default:
            break;
        }
      }
    }
  }

  private void ApplyInfiniteGameplayEffect(GameplayEffectInstance geInstance) {
    var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();

    foreach (var modifier in geInstance.GameplayEffect.EffectDefinition.Modifiers) {
      var magnitude = modifier.ModifierMagnitude.CalculateMagnitude(geInstance).GetValueOrDefault();
      var attributeModifier = new AttributeModifier();

      switch (modifier.ModifierType) {
        case AttributeModifierType.Add:
          attributeModifier.Add = magnitude;
          break;
        case AttributeModifierType.Multiply:
          attributeModifier.Multiply = magnitude;
          break;
        case AttributeModifierType.Override:
          attributeModifier.Override = magnitude;
          break;
        default:
          break;
      }

      modifiersToApply.Add(new() { Attribute = modifier.Attribute, Modifier = attributeModifier });
    }

    AppliedEffects.Add(new GameplayEffectContainer() { EffectInstance = geInstance, Modifiers = modifiersToApply.ToArray() });
  }

  public void CleanGameplayEffects() {
    AppliedEffects.RemoveAll(e => e.EffectInstance.GameplayEffect.EffectDefinition.DurationPolicy == DurationPolicy.Duration && e.EffectInstance.DurationRemaining <= 0);
  }
}

public class GameplayEffectContainer {
  public GameplayEffectInstance EffectInstance;
  public ModifierContainer[] Modifiers;

  public class ModifierContainer {
    public GameplayAttribute Attribute;
    public AttributeModifier Modifier;
  }
}