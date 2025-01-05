namespace HOB;

using System;
using GameplayFramework;
using Godot;
using Godot.Collections;
using HexGridMap;
using HOB.GameEntity;


// TODO: handle current player turns and entity managment
/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent, IGetGameState<IMatchGameState> {
  public Array<Entity> Entities { get; private set; }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public override void Init() {
    base.Init();

    Entities = new();
  }

  public void SpawnEntity(Entity entity, HexCoordinates coords) {
    entity.Ready += () => {
      entity.GlobalPosition = GetGameState().GameBoard.GetPoint(coords);
    };

    GetGameState().GameBoard.AddChild(entity);

    Entities.Add(entity);
  }
}
