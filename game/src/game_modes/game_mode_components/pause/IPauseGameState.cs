namespace HOB;

using GameplayFramework;

public interface IPauseGameState : IGameState {
  bool PauseGame { get; }
}
