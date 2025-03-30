namespace GameplayAbilitySystem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameplayTags;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameplayAbilitySystem : Node {
  [Signal] public delegate void GameplayEffectAppliedEventHandler(GameplayEffectInstance gameplayEffectInstance);
  [Signal] public delegate void GameplayEffectExecutedEventHandler(GameplayEffectInstance gameplayEffectInstance);
  [Signal] public delegate void GameplayEffectRemovedEventHandler(GameplayEffectInstance gameplayEffectInstance);

  [Signal] public delegate void GameplayAbilityGrantedEventHandler(GameplayAbilityInstance gameplayAbility);
  [Signal] public delegate void GameplayAbilityRevokedEventHandler(GameplayAbilityInstance gameplayAbility);
  [Signal] public delegate void GameplayAbilityActivatedEventHandler(GameplayAbilityInstance gameplayAbility);
  [Signal] public delegate void GameplayAbilityEndedEventHandler(GameplayAbilityInstance gameplayAbility);

  public event Action<Tag, GameplayEventData?>? GameplayEventRecieved;

  public TagContainer OwnedTags { get; private set; } = new();

  private List<GameplayEffectInstance> AppliedEffects { get; set; } = new();
  private List<GameplayAbilityInstance> GrantedAbilities { get; set; } = new();

  public AttributeSystem AttributeSystem { get; private set; }

  public GameplayAbilitySystem() {
    AttributeSystem = new();

    AddChild(AttributeSystem);
  }

  public override void _Ready() {
    GameplayEffectApplied += OnGameplayEffectApplied;
    GameplayEffectRemoved += OnGameplayEffectRemoved;

    OwnedTags.TagAdded += OnTagAdded;
  }

  public override void _Process(double delta) {
    Tick(new TimeTickContext((float)delta));
  }

  public void GrantAbility(GameplayAbilityInstance abilityInstance) {
    abilityInstance.Activated += () => EmitSignal(SignalName.GameplayAbilityActivated, abilityInstance);
    abilityInstance.Ended += () => {
      EmitSignal(SignalName.GameplayAbilityEnded, abilityInstance);
      if (abilityInstance.AbilityResource.RemoveOnEnd) {
        RevokeAbility(abilityInstance);
      }
    };


    abilityInstance.TreeExiting += () => {
      GrantedAbilities.Remove(abilityInstance);
      EmitSignal(SignalName.GameplayAbilityRevoked, abilityInstance);
    };

    GrantedAbilities.Add(abilityInstance);
    AddChild(abilityInstance);
  }

  public void RevokeAbility(GameplayAbilityInstance abilityInstance) {
    abilityInstance.QueueFree();
  }

  public void CancelAllAbilities() {
    foreach (var ability in GrantedAbilities.Where(a => a.IsActive)) {
      ability.CancelAbility();
    }
  }

  public void CancelAbilities(TagContainer withTags) {
    foreach (var ability in GrantedAbilities.Where(a => a.IsActive && a.AbilityResource.AbilityTags != null && a.AbilityResource.AbilityTags.HasAnyOfTags(withTags))) {
      ability.CancelAbility();
    }
  }

  public void CancelAbility(GameplayAbilityInstance abilityInstance) {
    abilityInstance.CancelAbility();
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

  public async Task<bool> TryActivateAbilityAsync(GameplayAbilityInstance abilityInsance, GameplayEventData? eventData = null) {
    var tcs = new TaskCompletionSource<bool>();

    void handleCompleted() {
      abilityInsance.Ended -= handleCompleted;
      tcs.TrySetResult(true);
    }

    abilityInsance.Ended += handleCompleted;

    if (!TryActivateAbility(abilityInsance, eventData)) {
      return false;
    }

    return await tcs.Task;
  }

  public void SendGameplayEvent(Tag tag, GameplayEventData? eventData = null) {
    GameplayEventRecieved?.Invoke(tag, eventData);

    foreach (var ability in GrantedAbilities) {
      if (ability.AbilityResource.AbilityTriggers != null) {
        if (ability.AbilityResource.AbilityTriggers.Any(t => t.TriggerSource == AbilityTriggerSourceType.GameplayEvent && t.TriggerTag == tag)) {
          TryActivateAbility(ability, eventData);
        }
      }
    }
  }

  public void ApplyGameplayEffectToSelf(GameplayEffectInstance geInstance) {
    geInstance.TreeExiting += () => {
      AppliedEffects.RemoveAll(e => e == geInstance);
      EmitSignal(SignalName.GameplayEffectRemoved, geInstance);
      if (geInstance.GameplayEffect.ExpireEffect != null) {
        var ei = geInstance.Source.MakeOutgoingInstance(geInstance.GameplayEffect.ExpireEffect, 0);
        ei.Target = geInstance.Target;
        ei.Target?.CallDeferred(nameof(ApplyGameplayEffectToSelf), ei);
      }
    };

    geInstance.Ready += () => {
      EmitSignal(SignalName.GameplayEffectApplied, geInstance);
      switch (geInstance.GameplayEffect?.EffectDefinition?.DurationPolicy) {
        case DurationPolicy.Duration or DurationPolicy.Infinite:
          geInstance.ExecutePeriodic += ApplyInstantGameplayEffect;
          ApplyDurationGameplayEffect(geInstance);
          break;
        case DurationPolicy.Instant:
          ApplyInstantGameplayEffect(geInstance);
          geInstance.QueueFree();
          break;
        default:
          break;
      }
    };

    AddChild(geInstance);
  }

  public void Tick(ITickContext tickContext) {
    foreach (var effect in AppliedEffects) {
      effect.Tick(tickContext);
    }
  }
  public GameplayEffectInstance MakeOutgoingInstance(GameplayEffectResource gameplayEffect, float level) {
    return GameplayEffectInstance.CreateNew(gameplayEffect, this, level);
  }

  private void ApplyInstantGameplayEffect(GameplayEffectInstance geInstance) {
    if (geInstance.GameplayEffect?.EffectDefinition?.Modifiers != null) {
      foreach (var modifier in geInstance.GameplayEffect.EffectDefinition.Modifiers) {
        if (modifier != null) {
          var attribute = modifier.Attribute;
          var magnitue = modifier.GetMagnitude(geInstance);

          if (attribute != null) {
            var value = AttributeSystem.GetAttributeCurrentValue(attribute);
            if (value != null) {
              switch (modifier?.ModifierType) {
                case AttributeModifierType.Add:
                  value += magnitue;
                  break;
                case AttributeModifierType.Multiply:
                  value *= magnitue;
                  break;
                case AttributeModifierType.Override:
                  value = magnitue;
                  break;
                default:
                  break;
              }

              // foreach (var attributeSet in AttributeSets) {
              //   attributeSet.PostGameplayEffectExecute(attribute, ref newValue);
              // }
              AttributeSystem.SetAttributeBaseValue(attribute, value ?? 0);
            }
          }
        }
      }
    }

    EmitSignal(SignalName.GameplayEffectExecuted, geInstance);
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
  private void OnTagAdded(Tag tag) {
    foreach (var ability in GrantedAbilities) {
      if (ability.AbilityResource.AbilityTriggers != null) {
        if (ability.AbilityResource.AbilityTriggers.Any(t => t.TriggerSource == AbilityTriggerSourceType.OwnedTagAdded && t.TriggerTag == tag)) {
          TryActivateAbility(ability);
        }
      }
    }
  }
}
