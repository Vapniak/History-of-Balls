namespace GameplayAbilitySystem;

public class TimeTickContext : TickContext {
  public float DeltaTime { get; private set; }
  public TimeTickContext(float deltaTime) {
    DeltaTime = deltaTime;
  }
}