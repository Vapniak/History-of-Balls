namespace GameplayFramework;

public interface IGameModeComponent<T> where T : IGameState {
  T GetGameState();
}
