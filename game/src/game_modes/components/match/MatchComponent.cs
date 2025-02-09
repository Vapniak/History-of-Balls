namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent {
  [Export] private PackedScene TestEntity { get; set; }
  [Export] private PackedScene TestEntity2 { get; set; }

  private GameBoard GameBoard { get; set; }

  private IMatchController _lastPlayer;
  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
    GetGameState().TurnChangedEvent += OnTurnChanged;
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(PlayerState playerState) {
    // TODO: spawning from map

    var controller = playerState.GetController<IMatchController>();
    controller.EndTurnEvent += () => OnEndTurn(controller);

    if (controller is PlayerController) {
      var entity = TestEntity.InstantiateOrNull<Entity>().Duplicate() as Entity;
      var entity2 = TestEntity2.InstantiateOrNull<Entity>().Duplicate() as Entity;
      var entity3 = TestEntity2.InstantiateOrNull<Entity>().Duplicate() as Entity;
      GameBoard.TryAddEntity(entity, new(0, 0), controller);
      GameBoard.TryAddEntity(entity2, new(1, 0), controller);
      GameBoard.TryAddEntity(entity3, new(2, 0), controller);
    }
    else {
      var entity = TestEntity.InstantiateOrNull<Entity>().Duplicate() as Entity;
      var entity2 = TestEntity2.InstantiateOrNull<Entity>().Duplicate() as Entity;
      GameBoard.TryAddEntity(entity, new(GameBoard.GetMapSize().X, GameBoard.GetMapSize().Y), controller);
      GameBoard.TryAddEntity(entity2, new(GameBoard.GetMapSize().X, GameBoard.GetMapSize().Y), controller);

      OnGameStarted();
    }
  }

  public void OnGameStarted() {
    foreach (var player in GetGameState().PlayerArray) {
      var controller = player.GetController<IMatchController>();
      controller.OnGameStarted();
    }
  }

  private void OnTurnChanged(int playerIndex) {
    _lastPlayer?.OwnTurnEnded();
    var player = GetGameState().PlayerArray[playerIndex].GetController<IMatchController>();
    player.OwnTurnStarted();
    _lastPlayer = player;
  }

  private void OnEndTurn(IMatchController controller) {
    if (GetGameState().IsCurrentTurn(controller)) {
      GetGameState().NextTurn();
    }
  }
}
