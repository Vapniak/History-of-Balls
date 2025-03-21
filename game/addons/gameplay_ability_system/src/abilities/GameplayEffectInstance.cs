namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class GameplayEffectInstance : Node {
  public GameplayEffectResource GameplayEffect { get; private set; }

  public float DurationRemaining { get; private set; }
  public float TotalDuration { get; private set; }
  public float TimeUntilNextPeriodTick { get; private set; }
  public float Level { get; private set; }
  public GameplayAbilitySystem Source { get; private set; }
  public GameplayAbilitySystem? Target { get; private set; }

  public GameplayAttributeValue? SourceCapturedValue;

  public static GameplayEffectInstance CreateNew(GameplayEffectResource gameplayEffect, GameplayAbilitySystem source, float level) {
    return new GameplayEffectInstance(gameplayEffect, source, level);
  }

  private GameplayEffectInstance(GameplayEffectResource gameplayEffect, GameplayAbilitySystem source, float level) {
    GameplayEffect = gameplayEffect;
    Source = source;
    Level = level;


    if (GameplayEffect.EffectDefinition != null) {
      if (GameplayEffect.EffectDefinition.Modifiers != null) {
        foreach (var modifier in GameplayEffect.EffectDefinition.Modifiers) {
          modifier?.ModifierMagnitude?.Initialize(this);
        }
      }

      if (GameplayEffect.EffectDefinition.DurationModifier != null) {
        DurationRemaining = (GameplayEffect.EffectDefinition.DurationModifier.CalculateMagnitude(this) * GameplayEffect.EffectDefinition.DurationMultiplier).GetValueOrDefault();
      }
    }

    if (GameplayEffect.Period != null) {
      if (GameplayEffect.Period.ExecuteOnApplication) {
        TimeUntilNextPeriodTick = 0;
      }
    }
  }

  public void SetTarget(GameplayAbilitySystem abilitySystem) {
    Target = abilitySystem;
  }

  public void SetTotalDuration(float totalDuration) {
    TotalDuration = totalDuration;
  }

  public void SetDuration(float duration) {
    DurationRemaining = duration;
  }

  public void UpdateRemainingDurationBy(float delta) {
    DurationRemaining -= delta;
  }

  public void SetLevel(float level) {
    Level = level;
  }

  public void TickPeriodic(float delta, out bool executePeriodic) {
    TimeUntilNextPeriodTick -= delta;
    executePeriodic = false;

    if (TimeUntilNextPeriodTick <= 0) {
      if (GameplayEffect.Period != null) {
        TimeUntilNextPeriodTick = GameplayEffect.Period.Period;

        if (GameplayEffect.Period.Period > 0) {
          executePeriodic = true;
        }
      }
    }
  }
}