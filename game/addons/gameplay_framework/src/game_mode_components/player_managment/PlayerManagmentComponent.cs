namespace GameplayFramework;

using Godot;
using Godot.Collections;
using System;

public partial class PlayerSpawnSettings : Resource {
  public PackedScene ControllerScene { get; private set; }
  public PlayerState PlayerState { get; private set; }
  public string PlayerName { get; private set; }
  public PackedScene HUDScene { get; private set; }
  public PackedScene CharacterScene { get; private set; }
  public PlayerSpawnSettings(PackedScene controller, PlayerState playerState, string playerName = "Player", PackedScene hud = null, PackedScene character = null) {
    ControllerScene = controller;
    PlayerState = playerState;
    PlayerName = playerName;
    HUDScene = hud;
    CharacterScene = character;
  }
}

[GlobalClass]
public partial class PlayerManagmentComponent : GameModeComponent {
  [Signal] public delegate void PlayerSpawnedEventHandler(PlayerState playerState);

  public override void _Ready() {
    base._Ready();

    GetGameState().PlayerArray = new();
  }

  public override IPlayerManagmentGameState GetGameState() => base.GetGameState() as IPlayerManagmentGameState;

  public void SpawnPlayerDeferred(PlayerSpawnSettings spawnSettings) {
    CallDeferred(MethodName.SpawnPlayer, spawnSettings);
  }

  protected virtual void SpawnPlayer(PlayerSpawnSettings settings) {
    var con = settings.ControllerScene.InstantiateOrNull<Controller>();
    var character = settings.CharacterScene?.InstantiateOrNull<Node>();
    var hud = settings.HUDScene?.InstantiateOrNull<HUD>();
    var parent = Game.GetWorld().CurrentLevel;

    if (character != null) {
      parent.AddChild(character);
      if (con is PlayerController playerController) {
        playerController.SetHUD(hud);
        playerController.SpawnHUD();

        if (character is IPlayerControllable controllable) {
          playerController.SetControllable(controllable);
        }
      }
    }

    var playerState = settings.PlayerState;
    playerState.SetPlayerName(settings.PlayerName);
    playerState.SetPlayerIndex(GetGameState().PlayerArray.Count);
    playerState.SetController(con);

    con.SetPlayerState(playerState);

    GetGameState().PlayerArray.Add(playerState);

    parent.AddChild(con);

    EmitSignal(SignalName.PlayerSpawned, playerState);
  }
}
