namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public partial class PlayerManagmentComponent : GameModeComponent, IGameModeComponent<IPlayerManagmentGameState> {
  [Export] public string DefaultPlayerName { get; set; } = "Player";

  [Export] public PackedScene PlayerScene { get; private set; }
  [Export] public PackedScene PlayerControllerScene { get; private set; }
  [Export] public PackedScene HUDScene { get; private set; }

  public override void Init() {
    base.Init();

    GetGameState().PlayerArray = new();

    CallDeferred(MethodName.SpawnPlayer);
  }
  public virtual void SpawnPlayer() {
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

      parent.AddChild(playerController);


      var playerState = CreatePlayerState();
      playerState.SetPlayerName(DefaultPlayerName);
      playerState.SetController(playerController);

      playerController.SetHUD(hud);
      playerController.SpawnHUD();
      playerController.SetPlayerState(playerState);

      GetGameState().PlayerArray.Add(playerState);
    }
  }

  protected virtual PlayerState CreatePlayerState() {
    return new PlayerState();
  }

  public IPlayerManagmentGameState GetGameState() => GameState as IPlayerManagmentGameState;
}
