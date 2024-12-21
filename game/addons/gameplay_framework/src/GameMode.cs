namespace GameplayFramework;

using Godot;

/// <summary>
/// Manages game logic.
/// </summary>
[GlobalClass]
public partial class GameMode : Node {
  [Export] public bool Pausable { get; set; }
  [Export] public string DefaultPlayerName { get; set; } = "Player";

  [Export] public PackedScene PlayerScene { get; private set; }
  [Export] public PlayerState PlayerState { get; private set; }
  [Export] public PackedScene PlayerControllerScene { get; private set; }

  [Export] public GameState GameState { get; private set; }

  [Export] public PackedScene HUDScene { get; private set; }


  public override void _Ready() {
    GameState = new();

    CallDeferred(MethodName.SpawnPlayer);
  }

  public void SpawnPlayer() {
    var player = PlayerScene?.InstantiateOrNull<Node>();
    var playerController = PlayerControllerScene?.InstantiateOrNull<PlayerController>();
    var hud = HUDScene?.InstantiateOrNull<HUD>();
    var world = Game.GetWorld();

    if (playerController != null) {
      world.AddChild(playerController);

      if (player != null) {
        world.AddChild(player);
        if (player is IPlayerControllable controllable) {
          controllable.PlayerController = playerController;
          playerController.SetControllable(controllable);
        }
      }

      PlayerState = new();
      PlayerState.SetPlayerName(DefaultPlayerName);
      PlayerState.SetController(playerController);

      playerController.SetHUD(hud);
      playerController.SpawnHUD();
      playerController.SetPlayerState(PlayerState);

      GameState.PlayerArray.Add(PlayerState);
    }
  }
}
