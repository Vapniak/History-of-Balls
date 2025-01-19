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

  private HexCell _lastSelectedCell;

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(PlayerState playerState) {
    // TODO: spawning from map
    var entity = TestEntity.InstantiateOrNull<Entity>();
    var entity2 = TestEntity.InstantiateOrNull<Entity>();
    GetGameState().GameBoard.AddEntity(entity, new(0, 0), playerState.GetController<IMatchController>());
    GetGameState().GameBoard.AddEntity(entity2, new(1, 0), playerState.GetController<IMatchController>());


    // TODO: add better highlighting system
    foreach (var e in playerState.GetController<IMatchController>().OwnedEntities) {
      GetGameState().GameBoard.AddHighlightToCells(new[] { e.Cell });
    }

    playerState.GetController<IMatchController>().CoordClicked += (coord) => OnCoordClicked(playerState.GetController<IMatchController>(), coord);
  }

  private void OnCoordClicked(IMatchController controller, CubeCoord coord) {
    var cell = GetGameState().GameBoard.GetCell(coord);

    DeselectCell(controller, _lastSelectedCell);
    if (cell == null) {

      return;
    }

    SelectCell(controller, cell);
  }

  private void SelectCell(IMatchController controller, HexCell cell) {
    var entities = GetGameState().GameBoard.GetOwnedEntitiesOnCell(controller, cell);
    foreach (var entity in entities) {
      if (entity.TryGetTrait<MoveTrait>(out var moveTrait)) {
        foreach (var c in moveTrait.GetCellsInMoveRange()) {
          // TODO: use terrain cost not range
          if (GetGameState().GameBoard.GetEntitiesOnCell(c).Length == 0) {
            GetGameState().GameBoard.AddHighlightToCells(new[] { c });
          }
        }
      }
    }

    GD.PrintS("Entities:", entities.Length, "Q:", cell.Coord.Q, "R:", cell.Coord.R);



    _lastSelectedCell = cell;
  }

  private void DeselectCell(IMatchController controller, HexCell cell) {
    var entities = GetGameState().GameBoard.GetOwnedEntitiesOnCell(controller, cell);
    foreach (var entity in entities) {
      if (entity.TryGetTrait<MoveTrait>(out var moveTrait)) {
        foreach (var c in moveTrait.GetCellsInMoveRange()) {
          if (GetGameState().GameBoard.GetEntitiesOnCell(c).Length == 0) {
            GetGameState().GameBoard.RemoveHighlightFromCells(new[] { c });
          }
        }
      }
    }
  }
}
