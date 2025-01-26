namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public partial class PlayerManagmentComponent : GameModeComponent {
  [Signal] public delegate void PlayerSpawnedEventHandler(PlayerState playerState);

  [Export] public string DefaultPlayerName { get; set; } = "Player";

  [Export] public bool AutoSpawn { get; private set; } = false;
  [Export] public PackedScene PlayerScene { get; private set; }
  [Export] public PackedScene PlayerControllerScene { get; private set; }
  [Export] public PackedScene HUDScene { get; private set; }


  public override void _Ready() {
    base._Ready();

    GetGameState().PlayerArray = new();
    if (AutoSpawn) {
      SpawnPlayerDeffered();
    }
  }
  public virtual void SpawnPlayerDeffered() {
    CallDeferred(MethodName.SpawnPlayer);
  }
  public override IPlayerManagmentGameState GetGameState() => base.GetGameState() as IPlayerManagmentGameState;

  public PlayerState GetLocalPlayer() => GetGameState().PlayerArray[0];


  private void SpawnPlayer() {
    var player = PlayerScene?.InstantiateOrNull<Node>();
    var playerController = PlayerControllerScene?.InstantiateOrNull<PlayerController>();
    var hud = HUDScene?.InstantiateOrNull<HUD>();
    var parent = Game.GetWorld().CurrentLevel;

    if (playerController != null) {
      if (player != null) {
        parent.AddChild(player);
        if (player is IPlayerControllable controllable) {
          playerController.SetControllable(controllable);
        }
      }



      var playerState = CreatePlayerState();
      playerState.SetPlayerName(DefaultPlayerName);
      playerState.SetController(playerController);

      playerController.SetHUD(hud);
      playerController.SpawnHUD();
      playerController.SetPlayerState(playerState);

      GetGameState().PlayerArray.Add(playerState);

      EmitSignal(SignalName.PlayerSpawned, playerState);
      player.AddChild(playerController);
    }
  }

  protected virtual PlayerState CreatePlayerState() {
    return new PlayerState();
  }

}
