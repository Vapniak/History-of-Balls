namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent {
  [Export] private EntityData TestEntity { get; set; }
  [Export] private EntityData TestEntity2 { get; set; }


  [Export] private ResourceType Primary { get; set; }
  [Export] private ResourceType Secondary { get; set; }

  private GameBoard GameBoard { get; set; }

  private IMatchController _lastPlayer;
  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
    GetGameState().TurnChangedEvent += OnTurnChanged;
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(IMatchPlayerState playerState) {
    // TODO: spawning from map

    var controller = playerState.GetController<IMatchController>();
    controller.EndTurnEvent += () => OnEndTurn(controller);

    playerState.PrimaryResourceType = Primary;
    playerState.SecondaryResourceType = Secondary;

    playerState.PrimaryResourceType.Value = 10;
    playerState.SecondaryResourceType.Value = 20;

    if (controller is PlayerController) {
      GameBoard.TryAddEntity(TestEntity, new(0, 0), controller);
      GameBoard.TryAddEntity(TestEntity, new(1, 0), controller);
      GameBoard.TryAddEntity(TestEntity2, new(2, 0), controller);
    }
    else {
      GameBoard.TryAddEntity(TestEntity, new(GameBoard.GetMapSize().X, GameBoard.GetMapSize().Y), controller);
      GameBoard.TryAddEntity(TestEntity2, new(GameBoard.GetMapSize().X, GameBoard.GetMapSize().Y), controller);

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
