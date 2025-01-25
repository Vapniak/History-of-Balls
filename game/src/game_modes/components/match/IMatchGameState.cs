namespace HOB;

using GameplayFramework;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public GameBoard GameBoard { get; }
}
