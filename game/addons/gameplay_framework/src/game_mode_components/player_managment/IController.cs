namespace GameplayFramework;


public interface IController : IGameplayFrameworkInterface {
  public void SetPlayerState(PlayerState playerState);
  public PlayerState GetPlayerState();
  public T? GetPlayerState<T>() where T : class, IPlayerState;
}
