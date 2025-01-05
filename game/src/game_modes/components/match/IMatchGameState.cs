namespace HOB;

using GameplayFramework;

public interface IMatchGameState : IGameState {
  GameBoard GameBoard { get; }
}
