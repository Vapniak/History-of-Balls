namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent {
  [Export] private Team PlayerTeam { get; set; }
  [Export] private Team AITeam { get; set; }

  [Export] private EntityData TestEntity { get; set; }
  [Export] private EntityData TestEntity2 { get; set; }
  [Export] private EntityData Structure1 { get; set; }
  [Export] private EntityData Factory { get; set; }
  [Export] private EntityData City { get; set; }


  [Export] private ResourceType Primary { get; set; }
  [Export] private ResourceType Secondary { get; set; }

  private GameBoard GameBoard { get; set; }

  private IMatchController _lastPlayer;
  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
    GetGameState().TurnStartedEvent += OnTurnStarted;
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(IMatchPlayerState playerState) {
    // TODO: spawning from map

    var controller = playerState.GetController<IMatchController>();
    controller.EndTurnEvent += () => OnEndTurn(controller);

    playerState.PrimaryResourceType = Primary.Duplicate() as ResourceType;
    playerState.SecondaryResourceType = Secondary.Duplicate() as ResourceType;

    if (controller is PlayerController) {
      controller.Team = PlayerTeam;
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity, new(1, 0), controller);
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity2, new(2, 0), controller);
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity, new(10, 5), controller);
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity2, new(2, 10), controller);

      GameBoard.AddEntityOnClosestAvailableCell(Structure1, new(5, 10), controller);
      GameBoard.AddEntityOnClosestAvailableCell(Factory, new(3, 3), controller);

      GameBoard.AddEntityOnClosestAvailableCell(City, new(5, 3), controller);
    }
    else {
      controller.Team = AITeam;
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity, new(GameBoard.GetMapSize().X - 5, GameBoard.GetMapSize().Y - 5), controller);
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity2, new(GameBoard.GetMapSize().X, GameBoard.GetMapSize().Y - 20), controller);
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity2, new(GameBoard.GetMapSize().X - 15, GameBoard.GetMapSize().Y - 6), controller);
      GameBoard.AddEntityOnClosestAvailableCell(TestEntity, new(GameBoard.GetMapSize().X - 10, GameBoard.GetMapSize().Y), controller);

      GameBoard.AddEntityOnClosestAvailableCell(Structure1, new(10, 10), controller);


      GameBoard.AddEntityOnClosestAvailableCell(Structure1, new(15, 10), null);
    }
  }

  public void OnGameStarted() {
    foreach (var player in GetGameState().PlayerArray) {
      var controller = player.GetController<IMatchController>();
      controller.OnGameStarted();
    }

    _lastPlayer = GetGameState().PlayerArray[GetGameState().CurrentPlayerIndex].GetController<IMatchController>();
  }

  private void OnTurnStarted() {
    _lastPlayer?.OwnTurnEnded();
    var player = GetGameState().PlayerArray[GetGameState().CurrentPlayerIndex].GetController<IMatchController>();
    player.OwnTurnStarted();
    _lastPlayer = player;
  }

  private void OnEndTurn(IMatchController controller) {
    if (GetGameState().IsCurrentTurn(controller)) {
      GetGameState().NextTurn();
    }
  }
}
