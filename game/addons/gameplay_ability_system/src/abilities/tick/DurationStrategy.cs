namespace GameplayAbilitySystem;

using Godot;

[GlobalClass, Tool]
public abstract partial class DurationStrategy : Resource {
  public abstract bool IsExpired { get; }
  public abstract void Initialize(float magnitude);
  public abstract void Left(float value);
  public abstract void Tick(ITickContext tickContext);
  public abstract void Reset();
}
