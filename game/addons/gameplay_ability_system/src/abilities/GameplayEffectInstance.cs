namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class GameplayEffectInstance : Node {
  [Signal] public delegate void ExecutePeriodicEventHandler(GameplayEffectInstance gameplayEffectInstance);
  public GameplayEffectResource GameplayEffect { get; private set; }

  public float Level { get; set; }
  public GameplayAbilitySystem Source { get; private set; }
  public GameplayAbilitySystem? Target { get; set; }

  private DurationStrategy? DurationStrategy { get; set; }
  private DurationStrategy? PeriodStrategy { get; set; }

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

      if (GameplayEffect.EffectDefinition.DurationStrategy != null) {
        DurationStrategy = GameplayEffect.EffectDefinition.DurationStrategy.Duplicate() as DurationStrategy;
        DurationStrategy?.Initialize((GameplayEffect.EffectDefinition.DurationModifier?.CalculateMagnitude(this) * GameplayEffect.EffectDefinition.DurationMultiplier).GetValueOrDefault());
      }
    }

    if (GameplayEffect.Period != null) {
      PeriodStrategy = GameplayEffect.Period.PeriodStrategy?.Duplicate() as DurationStrategy;
      PeriodStrategy?.Initialize(GameplayEffect.Period.Period);
      if (GameplayEffect.Period.ExecuteOnApplication) {
        GameplayEffect.Period?.PeriodStrategy?.Left(0);
      }
    }
  }

  public virtual void Execute() {

  }

  public virtual void Tick(TickContext tickContext) {
    if (GameplayEffect.EffectDefinition != null) {
      if (GameplayEffect?.Period?.PeriodStrategy != null) {
        GameplayEffect.Period.PeriodStrategy.Tick(tickContext);
        if (GameplayEffect.Period.PeriodStrategy.IsExpired) {
          EmitSignal(SignalName.ExecutePeriodic, this);
          GameplayEffect.Period.PeriodStrategy.Reset();
        }
      }

      if (GameplayEffect?.EffectDefinition.DurationPolicy == DurationPolicy.Duration && GameplayEffect?.EffectDefinition.DurationStrategy != null) {
        GameplayEffect.EffectDefinition.DurationStrategy.Tick(tickContext);
        if (GameplayEffect.EffectDefinition.DurationStrategy.IsExpired) {
          QueueFree();
        }
      }
    }
  }
}