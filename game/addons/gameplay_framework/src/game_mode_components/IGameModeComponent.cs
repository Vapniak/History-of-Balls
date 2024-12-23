namespace GameplayFramework;

public interface IGameModeComponent<T> where T : IGameState {
  public T GetGameState();
}
