namespace HOB;

using System;
using System.Collections.Generic;
using System.Linq;
using GameplayFramework;
using GameplayTags;
using Godot;
using HexGridMap;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
///

[GlobalClass]
public partial class MatchComponent : GameModeComponent, IMatchEvents, IEntityManagment {
  public event Action<Entity>? EntityAdded;
  public event Action<Entity>? EntityRemoved;
  public event Action<Tag>? MatchEvent;

  private List<Entity> Entities => GetGameState().Entities;

  private GameGrid Grid => GetGameState().GameBoard.Grid;

  private IMatchController? _lastPlayer;

  public override void _Ready() {
    base._Ready();

    GetGameState().Entities = new();
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(IMatchPlayerState playerState) {
    var controller = playerState.GetController<IMatchController>();
    controller.EndTurnEvent += () => OnEndTurn(controller);
  }

  public void OnGameStarted() {
    GetGameState().GameTimeMSec = 0;

    foreach (var player in GetGameState().PlayerArray) {
      var controller = player.GetController<IMatchController>();
      controller.OnGameStarted();
    }

    _lastPlayer = GetGameState().PlayerArray[GetGameState().CurrentPlayerIndex].GetController<IMatchController>();

    MatchEvent?.Invoke(TagManager.GetTag(HOBTags.EventGameStarted));

    MatchEvent?.Invoke(TagManager.GetTag(HOBTags.EventTurnStarted));
    OnTurnStarted();
  }

  public bool IsCurrentTurn(IMatchController controller) {
    return GetGameState().CurrentPlayerIndex == controller.GetPlayerState().PlayerIndex;
  }

  public void AddEntityOnClosestAvailableCell(EntityData data, OffsetCoord coord, IMatchController? owner) {
    GameCell? closestCell = null;
    var minDistance = int.MaxValue;

    var cube = Grid.GetLayout().OffsetToCube(coord);
    foreach (var cell in Grid.GetCells()) {
      var distance = cube.Distance(cell.Coord);
      if (distance < minDistance && CanEntityBePlacedOnCell(cell)) {
        minDistance = distance;
        closestCell = cell;
      }
    }

    if (closestCell != null) {
      var entity = data.CreateEntity(closestCell, this, owner);
      if (entity != null) {
        AddEntity(entity);
      }
    }
  }

  private bool CanEntityBePlacedOnCell(GameCell cell) {
    if (Grid.GetSetting(cell).IsWater || GetEntitiesOnCell(cell).Any(e => e.AbilitySystem.OwnedTags.HasExactTag(TagManager.GetTag(HOBTags.EntityTypeUnit)))) {
      return false;
    }

    return true;
  }

  public bool TryAddEntityOnCell(EntityData data, GameCell cell, IMatchController owner) {
    if (CanEntityBePlacedOnCell(cell)) {

      var entity = data.CreateEntity(cell, this, owner);
      if (entity != null) {
        AddEntity(entity);
        return true;
      }
    }

    return false;
  }


  private void AddEntity(Entity entity) {
    entity.TreeExiting += () => RemoveEntity(entity);
    entity.Ready += () => {
      EntityAdded?.Invoke(entity);
    };

    Entities.Add(entity);
    AddChild(entity);
  }

  public void RemoveEntity(Entity entity) {
    entity.QueueFree();
    Entities.Remove(entity);
    EntityRemoved?.Invoke(entity);
  }

  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, GameCell cell) {
    return GetOwnedEntites(owner).Where(e => e.Cell == cell).ToArray();
  }

  public Entity[] GetOwnedEntites(IMatchController owner) {
    return Entities.Where(e => e.TryGetOwner(out var o) && o == owner).ToArray();
  }


  public Entity[] GetNotOwnedEntities() {
    return Entities.Where(e => !e.TryGetOwner(out _)).ToArray();
  }

  public Entity[] GetEntitiesOnCell(GameCell cell) {
    return Entities.Where(e => e.Cell == cell).ToArray();
  }

  public Entity[] GetEnemyEntities(IMatchController controller) {
    return Entities.Where(e => e.TryGetOwner(out var owner) && owner != controller).ToArray();
  }

  public Entity[] GetEntities() => Entities.ToArray();

  private void OnTurnStarted() {
    _lastPlayer?.OwnTurnEnded();
    var player = GetGameState().PlayerArray[GetGameState().CurrentPlayerIndex].GetController<IMatchController>();

    player.OwnTurnStarted();

    _lastPlayer = player;
  }

  private void OnEndTurn(IMatchController controller) {
    if (IsCurrentTurn(controller)) {
      NextTurn();
    }
  }

  private void NextTurn() {
    MatchEvent?.Invoke(TagManager.GetTag(HOBTags.EventTurnEnded));

    GetGameState().CurrentPlayerIndex++;

    if (GetGameState().CurrentPlayerIndex >= GetGameState().PlayerArray.Count) {
      GetGameState().CurrentRound++;
      GetGameState().CurrentPlayerIndex = 0;
      // new round
    }
    MatchEvent?.Invoke(TagManager.GetTag(HOBTags.EventTurnStarted));
    OnTurnStarted();
  }

  public void TriggerGameEnd(IMatchController controller) {
    MatchEvent?.Invoke(TagManager.GetTag(HOBTags.EventGameEnded));
  }
}
