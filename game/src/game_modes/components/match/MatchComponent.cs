namespace HOB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent, IMatchEvents, IEntityManagment {
  public event Action TurnStarted;
  public event Action TurnChanged;
  public event Action TurnEnded;
  public event Action RoundStarted;
  public event Action<Entity> EntityAdded;
  public event Action<Entity> EntityRemoved;
  public event Action<IMatchController> GameEnded;

  [Export] private Country PlayerTeam { get; set; }
  [Export] private Country AITeam { get; set; }

  [Export] private EntityData Team1Infantry { get; set; }
  [Export] private EntityData Team1Ranged { get; set; }

  [Export] private EntityData Team2Infantry { get; set; }
  [Export] private EntityData Team2Ranged { get; set; }
  [Export] private EntityData Village { get; set; }
  [Export] private EntityData Factory { get; set; }
  [Export] private EntityData Team1City { get; set; }
  [Export] private EntityData Team2City { get; set; }


  [Export] private ResourceType Primary { get; set; }
  [Export] private ResourceType Secondary { get; set; }

  //public CommandManager CommandManager { get; private set; }

  private List<Entity> Entities => GetGameState().Entities;

  private GameGrid Grid => GetGameState().GameBoard.Grid;

  private IMatchController _lastPlayer;

  public override void _Ready() {
    base._Ready();

    GetGameState().Entities = new();

    TurnStarted += OnTurnStarted;
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(IMatchPlayerState playerState) {
    // TODO: spawning from map

    var controller = playerState.GetController<IMatchController>();
    controller.EndTurnEvent += () => OnEndTurn(controller);

    playerState.PrimaryResourceType = Primary.Duplicate() as ResourceType;
    playerState.SecondaryResourceType = Secondary.Duplicate() as ResourceType;

    playerState.PrimaryResourceType.Value = 5;

    if (controller is PlayerController) {
      controller.Country = PlayerTeam;
      AddEntityOnClosestAvailableCell(Team1Infantry, new(1, 0), controller);
      AddEntityOnClosestAvailableCell(Team1Ranged, new(1, 3), controller);
      AddEntityOnClosestAvailableCell(Team1Infantry, new(7, 5), controller);
      AddEntityOnClosestAvailableCell(Team1Ranged, new(2, 10), controller);
      AddEntityOnClosestAvailableCell(Team1Infantry, new(6, 7), controller);

      AddEntityOnClosestAvailableCell(Village, new(5, 2), controller);
      AddEntityOnClosestAvailableCell(Factory, new(2, 5), controller);

      AddEntityOnClosestAvailableCell(Team1City, new(2, 8), controller);
    }
    else {
      controller.Country = AITeam;
      AddEntityOnClosestAvailableCell(Team2Infantry, new(25, 5), controller);
      AddEntityOnClosestAvailableCell(Team2Ranged, new(25, 20), controller);
      AddEntityOnClosestAvailableCell(Team2Ranged, new(28, 6), controller);
      AddEntityOnClosestAvailableCell(Team2Infantry, new(22, 8), controller);
      AddEntityOnClosestAvailableCell(Team2Ranged, new(21, 7), controller);

      AddEntityOnClosestAvailableCell(Village, new(20, 10), controller);

      AddEntityOnClosestAvailableCell(Factory, new(17, 16), controller);

      AddEntityOnClosestAvailableCell(Team2City, new(20, 15), controller);
      AddEntityOnClosestAvailableCell(Village, new(15, 1), null);
      AddEntityOnClosestAvailableCell(Village, new(13, 8), null);
      AddEntityOnClosestAvailableCell(Village, new(10, 14), null);
    }
  }

  public void OnGameStarted() {
    GetGameState().GameStartTicksMSec = Time.GetTicksMsec();

    foreach (var player in GetGameState().PlayerArray) {
      var controller = player.GetController<IMatchController>();
      controller.OnGameStarted();
    }

    _lastPlayer = GetGameState().PlayerArray[GetGameState().CurrentPlayerIndex].GetController<IMatchController>();
  }

  public bool IsCurrentTurn(IMatchController controller) {
    return GetGameState().CurrentPlayerIndex == controller.GetPlayerState().PlayerIndex;
  }

  private void AddEntityOnClosestAvailableCell(EntityData data, OffsetCoord coord, IMatchController owner) {
    GameCell closestCell = null;
    var minDistance = int.MaxValue;

    var cube = Grid.GetLayout().OffsetToCube(coord);
    foreach (var cell in Grid.GetCells()) {
      var distance = cube.Distance(cell.Coord);
      if (distance < minDistance && CanEntityBePlacedOnCell(cell)) {
        minDistance = distance;
        closestCell = cell;
      }
    }

    var entity = new Entity(data, closestCell, this);

    AddEntity(entity, owner);
  }

  private bool CanEntityBePlacedOnCell(GameCell cell) {
    if (Grid.GetSetting(cell).IsWater) {
      return false;
    }

    foreach (var entity in GetEntitiesOnCell(cell)) {
      if (entity.TryGetTrait<ObstacleTrait>(out _)) {
        return false;
      }
    }

    return true;
  }

  public bool TryAddEntityOnCell(EntityData data, GameCell cell, IMatchController owner) {
    if (CanEntityBePlacedOnCell(cell)) {

      var entity = new Entity(data, cell, this);
      AddEntity(entity, owner);
      return true;
    }

    return false;
  }


  private void AddEntity(Entity entity, IMatchController owner) {
    entity.TreeExiting += () => RemoveEntity(entity);
    entity.Ready += () => {
      EntityAdded?.Invoke(entity);

      entity.ChangeOwner(owner);
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
    TurnEnded?.Invoke();

    GetGameState().CurrentPlayerIndex++;

    if (GetGameState().CurrentPlayerIndex >= GetGameState().PlayerArray.Count) {
      GetGameState().CurrentRound++;
      GetGameState().CurrentPlayerIndex = 0;
      RoundStarted?.Invoke();
    }

    TurnChanged?.Invoke();
    TurnStarted?.Invoke();
  }

  public void TriggerGameEnd(IMatchController controller) {
    GameEnded?.Invoke(controller);
  }
}
