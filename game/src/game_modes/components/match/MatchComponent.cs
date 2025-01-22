namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent {
  // TODO: handle current player turns

  [Export] private PackedScene TestEntity { get; set; }

  private IMatchController CurrentController { get; set; }
  private GameBoard GameBoard { get; set; }

  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
    CurrentController = GetGameState().PlayerArray[0].GetController<IMatchController>();
  }
  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(PlayerState playerState) {
    // TODO: spawning from map
    var entity = TestEntity.InstantiateOrNull<Entity>();
    var entity2 = TestEntity.InstantiateOrNull<Entity>();
    GameBoard.AddEntity(entity, new(0, 0), playerState.GetController<IMatchController>());
    GameBoard.AddEntity(entity2, new(1, 0), playerState.GetController<IMatchController>());

    playerState.GetController<IMatchController>().CoordClicked += (coord) => OnCoordClicked(playerState.GetController<IMatchController>(), coord);
  }

  private void OnCoordClicked(IMatchController controller, CubeCoord coord) {
    if (CurrentController != controller) {
      return;
    }

    var cell = GameBoard.GetCell(coord);

    if (cell == null) {
      return;
    }

    SelectCell(cell);
  }

  // TODO: selection system and showing actions
  private void SelectCell(GameCell cell) {
    var entities = GameBoard.GetOwnedEntitiesOnCell(CurrentController, cell);
    if (entities.Length > 0) {

    }
    GD.PrintS("Entities:", entities.Length, "Q:", cell.Coord.Q, "R:", cell.Coord.R);
  }
}
