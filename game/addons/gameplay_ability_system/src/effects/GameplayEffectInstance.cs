namespace GameplayAbilitySystem;

using System.Collections.Generic;
using GameplayTags;
using Godot;
using HOB;

[GlobalClass]
public partial class GameplayEffectInstance : Node {
  [Signal] public delegate void ExecutePeriodicEventHandler(GameplayEffectInstance gameplayEffectInstance);
  public GameplayEffectResource GameplayEffect { get; private set; }

  public float Level { get; set; }
  public GameplayAbilitySystem Source { get; private set; }
  public GameplayAbilitySystem Target { get; private set; }

  private DurationStrategy? DurationStrategy { get; set; }
  private DurationStrategy? PeriodStrategy { get; set; }

  private Dictionary<Tag, float> SetByCallers { get; set; } = new();

  public static GameplayEffectInstance CreateNew(GameplayEffectResource gameplayEffect, GameplayAbilitySystem source, GameplayAbilitySystem target, float level) {
    return new GameplayEffectInstance(gameplayEffect, source, target, level);
  }

  private GameplayEffectInstance(GameplayEffectResource gameplayEffect, GameplayAbilitySystem source, GameplayAbilitySystem target, float level) {
    GameplayEffect = gameplayEffect;
    Source = source;
    Level = level;
    Target = target;


    if (GameplayEffect.EffectDefinition != null) {
      if (GameplayEffect.EffectDefinition.AttributeModifiers != null) {
        foreach (var modifier in GameplayEffect.EffectDefinition.AttributeModifiers) {
          foreach (var mod in modifier.Modifiers) {
            mod?.ModifierMagnitude?.Initialize(this);
          }
        }
      }

      DurationStrategy = GameplayEffect.EffectDefinition.CreateDurationStrategy();
      DurationStrategy?.Initialize(GameplayEffect.EffectDefinition.GetDuration(this));
    }


    if (GameplayEffect.Period != null) {
      PeriodStrategy = GameplayEffect.Period.CreatePeriodStrategy();
      PeriodStrategy?.Initialize(GameplayEffect.Period.Period);
      if (GameplayEffect.Period.ExecuteOnApplication) {
        PeriodStrategy?.Left(0);
      }
    }

  }

  public virtual void Tick(ITickContext tickContext) {
    if (PeriodStrategy != null) {
      PeriodStrategy.Tick(tickContext);
      if (PeriodStrategy.IsExpired) {
        EmitSignal(SignalName.ExecutePeriodic, this);
        PeriodStrategy.Reset();
      }
    }

    if (GameplayEffect.EffectDefinition != null) {
      if (GameplayEffect?.EffectDefinition.DurationPolicy == DurationPolicy.Duration && DurationStrategy != null) {
        DurationStrategy.Tick(tickContext);
        if (DurationStrategy.IsExpired) {
          QueueFree();
        }
      }
    }
  }

  public void SetByCallerMagnitude(Tag tag, float magnitude) {
    SetByCallers[tag] = magnitude;
  }

  public float GetSetByCallerMagnitude(Tag tag, float defaultIfNotFound = 0) {
    return SetByCallers.GetValueOrDefault(tag, defaultIfNotFound);
  }


  public IEnumerator<(GameplayAttribute attribute, Aggregator aggregator)> GetAggregators() {
    var attributeModifiers = GameplayEffect.EffectDefinition?.AttributeModifiers;
    if (attributeModifiers != null) {
      foreach (var attrModifier in attributeModifiers) {
        if (attrModifier.Attribute != null) {
          var aggregator = new Aggregator();
          if (attrModifier.Modifiers != null) {
            foreach (var mod in attrModifier.Modifiers) {
              aggregator.AddMod(mod.ModifierType, mod.GetMagnitude(this));
            }
          }

          yield return (attrModifier.Attribute, aggregator);
        }
      }
    }
  }
}
