namespace GameplayAbilitySystem;

public class TimeTickContext : ITickContext {
  public float DeltaTime { get; private set; }
  public TimeTickContext(float deltaTime) {
    DeltaTime = deltaTime;
  }
}
