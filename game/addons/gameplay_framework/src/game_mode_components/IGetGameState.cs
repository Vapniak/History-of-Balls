namespace GameplayFramework;

public interface IGetGameState<T> where T : IGameState {
  T GetGameState();
}
