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

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(PlayerState playerState) {
    // TODO: spawning from map
    var entity = TestEntity.InstantiateOrNull<Entity>();
    var entity2 = TestEntity.InstantiateOrNull<Entity>();
    GetGameState().GameBoard.AddEntity(entity, new(0, 0), playerState.GetController<IMatchController>());
    GetGameState().GameBoard.AddEntity(entity2, new(1, 0), playerState.GetController<IMatchController>());


    // TODO: add better highlighting system
    foreach (var e in playerState.GetController<IMatchController>().OwnedEntities) {
      GetGameState().GameBoard.AddHighlightToCoords(new[] { e.Cell });
    }

    playerState.GetController<IMatchController>().CellSelected += (coords) => OnCellCelected(playerState.GetController<IMatchController>(), coords);
  }

  private void OnCellCelected(IMatchController controller, HexCell cell) {
    var entities = GetGameState().GameBoard.GetOwnedEntitiesOnCell(controller, cell);


    // TODO: entity selection

    GD.PrintS("Entities: " + entities.Length, "Q: " + cell.Coord.Q, "R: ", cell.Coord.R);
  }
}
