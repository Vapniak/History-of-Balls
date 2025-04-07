namespace GameplayAbilitySystem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameplayTags;
using Godot;

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
  public AttributeSystem AttributeSystem { get; private set; }

  public TagContainer OwnedTags { get; private set; } = new();
  private List<GameplayEffectInstance> AppliedEffects { get; set; } = new();
  private List<GameplayAbilityInstance> GrantedAbilities { get; set; } = new();



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
    EmitSignal(SignalName.GameplayAbilityGranted, abilityInstance);
  }

  public void RevokeAbility(GameplayAbilityInstance abilityInstance) {
    abilityInstance.QueueFree();
    EmitSignal(SignalName.GameplayAbilityRevoked, abilityInstance);
  }

  public T? GetGrantedAbility<T>() where T : GameplayAbilityInstance {
    return GrantedAbilities.OfType<T>().FirstOrDefault();
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
        var ei = geInstance.Source.MakeOutgoingInstance(geInstance.GameplayEffect.ExpireEffect, 0, geInstance.Target);
        ei.Target.CallDeferred(nameof(ApplyGameplayEffectToSelf), ei);
      }
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
        geInstance.QueueFree();
        break;
      default:
        break;
    }
  }

  public void Tick(ITickContext tickContext) {
    foreach (var effect in AppliedEffects) {
      effect.Tick(tickContext);
    }
  }
  public GameplayEffectInstance MakeOutgoingInstance(GameplayEffectResource gameplayEffect, float level, GameplayAbilitySystem? target = null) {
    return GameplayEffectInstance.CreateNew(gameplayEffect, this, target ?? this, level);
  }

  private void ApplyInstantGameplayEffect(GameplayEffectInstance geInstance) {
    var aggregators = GetAggregators(geInstance);
    while (aggregators.MoveNext()) {
      var attribute = aggregators.Current.Item1;
      var aggregator = aggregators.Current.Item2;
      var baseValue = AttributeSystem.GetAttributeBaseValue(attribute).GetValueOrDefault();
      var value = aggregator.Evaluate(baseValue);
      var data = new GameplayEffectModData(
        geInstance,
        // FIXME: attribute magnitude should not be this way?
        new(aggregators.Current.Item1, value - baseValue),
        geInstance.Target
      );

      foreach (var attributeSet in AttributeSystem.GetAttributeSets()) {
        attributeSet.PreGameplayEffectExecute(data);
      }

      AttributeSystem.SetAttributeBaseValue(attribute, value);

      UpdateCurrentValues();

      foreach (var attributeSet in AttributeSystem.GetAttributeSets()) {
        attributeSet.PostGameplayEffectExecute(data);
      }
    }


    EmitSignal(SignalName.GameplayEffectExecuted, geInstance);
  }

  private void ApplyDurationGameplayEffect(GameplayEffectInstance geInstance) {
    AppliedEffects.Add(geInstance);
    UpdateCurrentValues();
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

    UpdateCurrentValues();
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

  // private Dictionary<GameplayAttribute, AttributeModifier> GetModifiers(GameplayEffectInstance gameplayEffect) {
  //   var modifiersToApply = new Dictionary<GameplayAttribute, AttributeModifier>();
  //   if (gameplayEffect.GameplayEffect?.EffectDefinition?.Modifiers != null) {
  //     foreach (var modifier in gameplayEffect.GameplayEffect.EffectDefinition.Modifiers) {
  //       var attributeModifier = new AttributeModifier();
  //       var magnitude = modifier.GetMagnitude(gameplayEffect);

  //       switch (modifier.ModifierType) {
  //         case AttributeModifierType.Add:
  //           attributeModifier.Add = magnitude;
  //           break;
  //         case AttributeModifierType.Multiply:
  //           attributeModifier.Multiply = magnitude;
  //           break;
  //         case AttributeModifierType.Override:
  //           attributeModifier.Override = magnitude;
  //           break;
  //         default:
  //           break;
  //       }

  //       if (modifier.Attribute != null) {
  //         if (modifiersToApply.TryGetValue(modifier.Attribute, out var value)) {
  //           value = value.Combine(attributeModifier);
  //         }
  //         else {
  //           modifiersToApply.Add(modifier.Attribute, attributeModifier);
  //         }
  //       }
  //     }
  //   }

  //   return modifiersToApply;
  // }

  private IEnumerator<(GameplayAttribute, Aggregator)> GetAggregators(GameplayEffectInstance effectInstance) {
    var aggregator = new Aggregator();

    var modifiers = effectInstance.GameplayEffect?.EffectDefinition?.Modifiers;
    if (modifiers != null) {
      foreach (var modifier in modifiers) {
        // FIXME: what if i have 2 modifiers which are after another?
        aggregator.AddMod(modifier.ModifierType, modifier.GetMagnitude(effectInstance));

        if (modifier.Attribute != null) {
          yield return (modifier.Attribute, aggregator);
        }
      }
    }
  }

  private void UpdateCurrentValues() {
    foreach (var attribute in AttributeSystem.GetAllAttributes()) {
      var oldValue = AttributeSystem.GetAttributeCurrentValue(attribute).GetValueOrDefault();
      var value = AttributeSystem.GetAttributeBaseValue(attribute).GetValueOrDefault();

      foreach (var set in AttributeSystem.GetAttributeSets()) {
        set.PreAttributeChange(AttributeSystem, attribute, ref value);
      }

      AttributeSystem.SetAttributeCurrentValue(attribute, value);

      foreach (var set in AttributeSystem.GetAttributeSets()) {
        set.PostAttributeChange(AttributeSystem, attribute, oldValue, value);
      }
    }
  }
}
