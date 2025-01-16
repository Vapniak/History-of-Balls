namespace GameplayFramework;

public interface IGetGameState<T> where T : IGameState {
  public T GetGameState();
}
