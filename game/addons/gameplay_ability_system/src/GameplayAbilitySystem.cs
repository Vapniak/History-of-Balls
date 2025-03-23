namespace GameplayAbilitySystem;

using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTags;
using Godot;
using Godot.Collections;
using HOB;
using HOB.GameEntity;

[GlobalClass]
public partial class GameplayAbilitySystem : Node {
  [Signal] public delegate void AttributeValueChangedEventHandler(GameplayAttribute attribute, float oldValue, float newValue);
  [Signal] public delegate void GameplayEffectAppliedEventHandler(GameplayEffectInstance gameplayEffectInstance);
  [Signal] public delegate void GameplayEffectExecutedEventHandler(GameplayEffectInstance gameplayEffectInstance);
  [Signal] public delegate void GameplayEffectRemovedEventHandler(GameplayEffectInstance gameplayEffectInstance);

  [Signal] public delegate void GameplayAbilityGrantedEventHandler(GameplayAbilityInstance gameplayAbility);
  [Signal] public delegate void GameplayAbilityActivatedEventHandler(GameplayAbilityInstance gameplayAbility);
  [Signal] public delegate void GameplayAbilityEndedEventHandler(GameplayAbilityInstance gameplayAbility);


  [Export] private Array<GameplayAttributeSet> AttributeSets { get; set; } = new();

  public TagContainer OwnedTags { get; private set; } = new();


  private Godot.Collections.Dictionary<GameplayAttribute, GameplayAttributeValue> AttributeValues { get; set; } = new();
  private List<GameplayEffectContainer> AppliedEffects { get; set; } = new();
  private List<GameplayAbilityInstance> GrantedAbilities { get; set; } = new();

  public override void _Ready() {
    GameplayEffectApplied += OnGameplayEffectApplied;
    GameplayEffectRemoved += OnGameplayEffectRemoved;
  }

  public override void _Process(double delta) {
    Tick(new TimeTickContext((float)delta));
  }

  public void GrantAbility(GameplayAbilityInstance abilityInstance) {
    abilityInstance.Activated += () => EmitSignal(SignalName.GameplayAbilityActivated, abilityInstance);
    GrantedAbilities.Add(abilityInstance);
    AddChild(abilityInstance);
  }

  public IEnumerable<GameplayAbilityInstance> GetGrantedAbilities() => GrantedAbilities;

  public bool TryApplyGameplayEffectToSelf(GameplayEffectInstance geInstance) {
    if (geInstance == null) {
      return false;
    }

    geInstance.TreeExiting += () => {
      AppliedEffects.RemoveAll(e => e.EffectInstance == geInstance);
      EmitSignal(SignalName.GameplayEffectRemoved, geInstance);
    };

    AddChild(geInstance);

    switch (geInstance.GameplayEffect?.EffectDefinition?.DurationPolicy) {
      case DurationPolicy.Duration or DurationPolicy.Infinite:
        geInstance.ExecutePeriodic += ApplyInstantGameplayEffect;
        ApplyDurationGameplayEffect(geInstance);
        break;
      case DurationPolicy.Instant:
        ApplyInstantGameplayEffect(geInstance);
        break;
      default:
        break;
    }

    EmitSignal(SignalName.GameplayEffectApplied, geInstance);

    return true;
  }

  public void AddAttributeSet(GameplayAttributeSet attributeSet) {
    if (attributeSet == null) {
      return;
    }

    AttributeSets.Add(attributeSet);

    foreach (var attribute in attributeSet.GetAttributes()) {
      AttributeValues.TryAdd(attribute, new() { BaseValue = 5 });
    }
  }

  public void RemoveAttributeSet(GameplayAttributeSet attributeSet) {
    AttributeSets.Remove(attributeSet);
    foreach (var attribute in attributeSet.GetAttributes()) {
      AttributeValues.Remove(attribute);
    }
  }

  public IEnumerable<GameplayAttribute> GetAllAttributes() {
    return AttributeValues.Keys;
  }

  public bool TryGetAttributeSet<T>(out T? attributeSet) where T : GameplayAttributeSet {
    attributeSet = AttributeSets.OfType<T>().FirstOrDefault();

    return attributeSet != null;
  }

  public void Tick(TickContext tickContext) {
    foreach (var effect in AppliedEffects) {
      var ge = effect.EffectInstance;

      ge.Tick(tickContext);
    }
  }

  public float? GetAttributeCurrentValue(GameplayAttribute attribute) {
    return CalculateAttributeCurrentValue(attribute);
  }

  public GameplayEffectInstance MakeOutgoingInstance(GameplayEffectResource gameplayEffect, float level) {
    return GameplayEffectInstance.CreateNew(gameplayEffect, this, level);
  }

  private void ApplyInstantGameplayEffect(GameplayEffectInstance geInstance) {
    if (geInstance.GameplayEffect?.EffectDefinition?.Modifiers != null) {
      foreach (var modifier in geInstance.GameplayEffect.EffectDefinition.Modifiers) {
        if (modifier != null) {
          var attribute = modifier.Attribute;
          var magnitue = (modifier.ModifierMagnitude == null ? 1 : modifier.ModifierMagnitude.CalculateMagnitude(geInstance)) * modifier.Coefficient;

          if (attribute != null) {
            if (TryGetAttributeValue(attribute, out var value)) {
              if (value != null) {
                var oldValue = value.BaseValue;
                switch (modifier?.ModifierType) {
                  case AttributeModifierType.Add:
                    value.BaseValue += magnitue;
                    break;
                  case AttributeModifierType.Multiply:
                    value.BaseValue *= magnitue;
                    break;
                  case AttributeModifierType.Override:
                    value.BaseValue = magnitue;
                    break;
                  default:
                    break;
                }

                var newValue = value.BaseValue;

                EmitSignal(SignalName.AttributeValueChanged, attribute, oldValue, newValue);
              }
            }
          }
        }
      }

      EmitSignal(SignalName.GameplayEffectExecuted, geInstance);

      geInstance.QueueFree();
    }
  }

  private void ApplyDurationGameplayEffect(GameplayEffectInstance geInstance) {
    var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();
    if (geInstance.GameplayEffect?.EffectDefinition?.Modifiers != null) {
      foreach (var modifier in geInstance.GameplayEffect.EffectDefinition.Modifiers) {
        var attributeModifier = new AttributeModifier();
        var magnitude = (modifier?.ModifierMagnitude?.CalculateMagnitude(geInstance) * modifier?.Coefficient).GetValueOrDefault();

        switch (modifier?.ModifierType) {
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

        if (modifier?.Attribute != null) {
          modifiersToApply.Add(new() { Attribute = modifier.Attribute, Modifier = attributeModifier });
        }
      }
    }

    AppliedEffects.Add(new GameplayEffectContainer() { EffectInstance = geInstance, Modifiers = modifiersToApply.ToArray() });
  }

  private void OnGameplayEffectApplied(GameplayEffectInstance gameplayEffect) {
    if (gameplayEffect.GameplayEffect?.EffectDefinition?.DurationPolicy is DurationPolicy.Duration or DurationPolicy.Infinite && gameplayEffect.GameplayEffect.GrantedTags != null) {
      OwnedTags.AddTags(gameplayEffect.GameplayEffect.GrantedTags);
    }
  }

  private void OnGameplayEffectRemoved(GameplayEffectInstance gameplayEffect) {
    if (gameplayEffect.GameplayEffect?.EffectDefinition?.DurationPolicy is DurationPolicy.Duration or DurationPolicy.Infinite && gameplayEffect.GameplayEffect.GrantedTags != null) {
      OwnedTags.RemoveTags(gameplayEffect.GameplayEffect.GrantedTags);
    }
  }

  private float CalculateAttributeCurrentValue(GameplayAttribute attribute) {
    var value = AttributeValues[attribute];

    // TODO: apply modifiers

    return value.BaseValue;
  }

  private bool TryGetAttributeValue(GameplayAttribute attribute, out GameplayAttributeValue? value) {
    return AttributeValues.TryGetValue(attribute, out value);
  }
}

public class GameplayEffectContainer {
  public required GameplayEffectInstance EffectInstance;
  public required ModifierContainer[] Modifiers;

  public class ModifierContainer {
    public required GameplayAttribute Attribute;
    public required AttributeModifier Modifier;
  }
}
