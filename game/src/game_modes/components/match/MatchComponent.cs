namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;


// TODO: handle current player turns and entity managment
/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent, IGetGameState<IMatchGameState> {
  [Export] private PackedScene TestEntity { get; set; }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(PlayerState playerState) {
    // TODO: spawning from map
    var entity = TestEntity.InstantiateOrNull<Entity>();
    var entity2 = TestEntity.InstantiateOrNull<Entity>();
    GetGameState().GameBoard.EntityManager.AddEntity(entity, new(0, 0), playerState.GetController<IMatchController>());
    GetGameState().GameBoard.EntityManager.AddEntity(entity2, new(1, 0), playerState.GetController<IMatchController>());

    playerState.GetController<IMatchController>().CellSelected += (coords) => OnCellCelected(playerState.GetController<IMatchController>(), coords);
  }

  private void OnCellCelected(IMatchController controller, HexCoordinates coords) {
    var entities = GetGameState().GameBoard.EntityManager.GetOwnedEntitiesOnCoords(controller, coords);

    GetGameState().GameBoard.HighlightCells(new[] { coords });
    // TODO: entity selection
    var offset = coords.Roffset(Offset.Even);
    GD.PrintS(controller.GetPlayerState().PlayerName, entities.Count, "Q: " + coords.Q, "R: " + coords.R, "COL: " + offset.Col, "ROW: " + offset.Row);
  }
}
