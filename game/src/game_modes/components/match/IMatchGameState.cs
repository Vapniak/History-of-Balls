namespace HOB;

using GameplayFramework;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  GameBoard GameBoard { get; }
}
