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

  [Export] private Country PlayerTeam { get; set; }
  [Export] private Country AITeam { get; set; }

  [Export] private EntityData TestEntity { get; set; }
  [Export] private EntityData TestEntity2 { get; set; }
  [Export] private EntityData Structure1 { get; set; }
  [Export] private EntityData Factory { get; set; }
  [Export] private EntityData City { get; set; }


  [Export] private ResourceType Primary { get; set; }
  [Export] private ResourceType Secondary { get; set; }

  private List<Entity> Entities { get; set; }
  private string _entityUISceneUID = "uid://ka4lyslghbk";

  private GameGrid Grid => GetGameState().GameBoard.Grid;

  private IMatchController _lastPlayer;

  public override void _Ready() {
    base._Ready();

    Entities = new();

    TurnStarted += OnTurnStarted;
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(IMatchPlayerState playerState) {
    // TODO: spawning from map

    var controller = playerState.GetController<IMatchController>();
    controller.EndTurnEvent += () => OnEndTurn(controller);

    playerState.PrimaryResourceType = Primary.Duplicate() as ResourceType;
    playerState.SecondaryResourceType = Secondary.Duplicate() as ResourceType;

    if (controller is PlayerController) {
      controller.Country = PlayerTeam;
      AddEntityOnClosestAvailableCell(TestEntity, new(1, 0), controller);
      AddEntityOnClosestAvailableCell(TestEntity2, new(2, 0), controller);
      AddEntityOnClosestAvailableCell(TestEntity, new(10, 5), controller);
      AddEntityOnClosestAvailableCell(TestEntity2, new(2, 10), controller);

      AddEntityOnClosestAvailableCell(Structure1, new(5, 10), controller);
      AddEntityOnClosestAvailableCell(Factory, new(3, 3), controller);

      AddEntityOnClosestAvailableCell(City, new(5, 3), controller);
    }
    else {
      controller.Country = AITeam;
      AddEntityOnClosestAvailableCell(TestEntity, new(20, 5), controller);
      AddEntityOnClosestAvailableCell(TestEntity2, new(20, 20), controller);
      AddEntityOnClosestAvailableCell(TestEntity2, new(18, 6), controller);
      AddEntityOnClosestAvailableCell(TestEntity, new(17, 8), controller);
      AddEntityOnClosestAvailableCell(TestEntity2, new(15, 7), controller);
      AddEntityOnClosestAvailableCell(TestEntity, new(15, 10), controller);

      AddEntityOnClosestAvailableCell(Structure1, new(10, 10), controller);


      AddEntityOnClosestAvailableCell(Structure1, new(15, 10), null);
    }
  }

  public void OnGameStarted() {
    foreach (var player in GetGameState().PlayerArray) {
      var controller = player.GetController<IMatchController>();
      controller.OnGameStarted();
    }

    _lastPlayer = GetGameState().PlayerArray[GetGameState().CurrentPlayerIndex].GetController<IMatchController>();
  }

  public bool IsCurrentTurn(IMatchController controller) {
    return GetGameState().CurrentPlayerIndex == controller.GetPlayerState().PlayerIndex;
  }

  private void AddEntityOnClosestAvailableCell(EntityData data, CubeCoord coord, IMatchController owner) {
    GameCell closestCell = null;
    var minDistance = int.MaxValue;

    foreach (var cell in Grid.GetCells()) {
      var distance = coord.Distance(cell.Coord);
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
    AddEntity(entity);
    entity.ChangeOwner(owner);
    EntityAdded?.Invoke(entity);
  }

  private void AddEntity(Entity entity) {
    entity.TreeExiting += () => RemoveEntity(entity);

    var entityUI = ResourceLoader.Load<PackedScene>(_entityUISceneUID).Instantiate<EntityUi3D>();

    entityUI.SetVisible(false);

    entity.OwnerControllerChanged += () => {
      if (entity.TryGetOwner(out var owner)) {
        entityUI.SetFlag(owner.Country.Flag);
        entityUI.SetVisible(true);
      }
      else {
        entityUI.SetVisible(false);
      }
    };

    AddChild(entity);

    entity.Body.AddChild(entityUI);

    Entities.Add(entity);
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
}
