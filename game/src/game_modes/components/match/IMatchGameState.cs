namespace HOB;

using GameplayFramework;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public int CurrentPlayerIndex { get; set; }
  public int CurrentRound { get; set; }

  public GameBoard GameBoard { get; set; }
}
