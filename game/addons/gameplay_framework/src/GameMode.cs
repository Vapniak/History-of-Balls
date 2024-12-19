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
    if (player == null) {
      GD.PrintErr("Player is null");
      return;
    }

    var playerController = PlayerControllerScene?.InstantiateOrNull<PlayerController>();
    if (playerController == null) {
      GD.PrintErr("Player controller is null");
      return;
    }

    var hud = HUDScene?.InstantiateOrNull<HUD>();
    if (player != null && playerController != null) {
      var world = Game.GetWorld();
      world.AddChild(player);
      world.AddChild(playerController);

      PlayerState = new();
      GameState.PlayerArray.Add(PlayerState);

      playerController.SetPlayerState(PlayerState);
      playerController.SetHUD(hud);
      playerController.SpawnHUD();
    }
  }
}
