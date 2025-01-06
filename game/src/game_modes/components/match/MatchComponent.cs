namespace HOB;

using System.Collections.Generic;
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
    GetGameState().GameBoard.AddEntity(playerState, entity, new(0, 0));
    GetGameState().GameBoard.AddEntity(playerState, entity2, new(1, 0));

    playerState.GetController<TestPlayerController>().CellSelected += (coords) => OnCellCelected(playerState, coords);
  }

  private void OnCellCelected(PlayerState playerState, HexCoordinates coords) {
    var entities = GetGameState().GameBoard.GetEntitiesOnCoords(playerState, coords);

    GD.PrintS(playerState.PlayerName, entities.Count);
  }
}
