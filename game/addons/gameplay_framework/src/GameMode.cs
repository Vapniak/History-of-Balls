namespace GameplayFramework;

using Godot;
using HOB;

/// <summary>
/// Manages game logic.
/// </summary>
[GlobalClass]
public partial class GameMode : Node {
  [Export] public bool Pausable { get; set; }
  [Export] public string DefaultPlayerName { get; set; } = "Player";

  [Export] public PackedScene PlayerScene { get; private set; }
  [Export] public PackedScene PlayerControllerScene { get; private set; }
  [Export] public PackedScene HUDScene { get; private set; }

  private GameState GameState { get; set; }


  public override void _Ready() {
    GameState = CreateGameState();

    CallDeferred(MethodName.SpawnPlayer);
  }

  public void SpawnPlayer() {
    var player = PlayerScene?.InstantiateOrNull<Node>();
    var playerController = PlayerControllerScene?.InstantiateOrNull<PlayerController>();
    var hud = HUDScene?.InstantiateOrNull<HUD>();
    var world = Game.GetWorld();

    if (playerController != null) {
      if (player != null) {
        world.AddChild(player);
        if (player is IPlayerControllable controllable) {
          playerController.SetControllable(controllable);
        }
      }

      world.AddChild(playerController);


      var playerState = CreatePlayerState();
      playerState.SetPlayerName(DefaultPlayerName);
      playerState.SetController(playerController);

      playerController.SetHUD(hud);
      playerController.SpawnHUD();
      playerController.SetPlayerState(playerState);

      GameState.PlayerArray.Add(playerState);
    }
  }

  protected virtual PlayerState CreatePlayerState() {
    return new PlayerState();
  }

  protected virtual GameState CreateGameState() {
    return new GameState();
  }

  public T GetGameState<T>() where T : GameState => GetGameState() as T;
  public GameState GetGameState() => GameState;
}
