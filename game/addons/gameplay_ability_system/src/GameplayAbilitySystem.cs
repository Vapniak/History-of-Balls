namespace GameplayAbilitySystem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
  private List<GameplayEffectInstance> AppliedEffects { get; set; } = new();
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
    abilityInstance.Ended += () => EmitSignal(SignalName.GameplayAbilityEnded, abilityInstance);
    GrantedAbilities.Add(abilityInstance);
    AddChild(abilityInstance);
  }

  public IEnumerable<GameplayAbilityInstance> GetGrantedAbilities() => GrantedAbilities;

  public bool TryActivateAbility(GameplayAbilityInstance abilityInstance, GameplayEventData? eventData = null) {
    if (!abilityInstance.CanActivateAbility(eventData)) {
      return false;
    }

    abilityInstance.PreActivate(eventData);
    abilityInstance.ActivateAbility(eventData);

    return true;
  }

  public void TryApplyGameplayEffectToSelf(GameplayEffectInstance geInstance) {
    geInstance.TreeExiting += () => {
      AppliedEffects.RemoveAll(e => e == geInstance);
      EmitSignal(SignalName.GameplayEffectRemoved, geInstance);
    };


    AddChild(geInstance);

    EmitSignal(SignalName.GameplayEffectApplied, geInstance);
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

  public void Tick(ITickContext tickContext) {
    foreach (var effect in AppliedEffects) {
      effect.Tick(tickContext);
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

      if (geInstance.GameplayEffect.EffectDefinition.DurationPolicy == DurationPolicy.Instant) {
        QueueFree();
      }
    }
  }

  private void ApplyDurationGameplayEffect(GameplayEffectInstance geInstance) {
    // var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();
    // if (geInstance.GameplayEffect?.EffectDefinition?.Modifiers != null) {
    //   foreach (var modifier in geInstance.GameplayEffect.EffectDefinition.Modifiers) {
    //     var attributeModifier = new AttributeModifier();
    //     var magnitude = (modifier?.ModifierMagnitude?.CalculateMagnitude(geInstance) * modifier?.Coefficient).GetValueOrDefault();

    //     switch (modifier?.ModifierType) {
    //       case AttributeModifierType.Add:
    //         attributeModifier.Add = magnitude;
    //         break;
    //       case AttributeModifierType.Multiply:
    //         attributeModifier.Multiply = magnitude;
    //         break;
    //       case AttributeModifierType.Override:
    //         attributeModifier.Override = magnitude;
    //         break;
    //       default:
    //         break;
    //     }

    //     if (modifier?.Attribute != null) {
    //       modifiersToApply.Add(new() { Attribute = modifier.Attribute, Modifier = attributeModifier });
    //     }
    //   }
    // }
    AppliedEffects.Add(geInstance);
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
