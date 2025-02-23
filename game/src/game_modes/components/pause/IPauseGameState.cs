namespace HOB;

using System;
using GameplayFramework;

public interface IPauseGameState : IGameState {
  public event Action PausedEvent;
  public event Action ResumedEvent;
  public bool PauseGame { get; }
}
