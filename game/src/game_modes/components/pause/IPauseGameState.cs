namespace HOB;

using GameplayFramework;

public interface IPauseGameState : IGameState {
  public bool PauseGame { get; }
}
