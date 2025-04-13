namespace GameplayFramework;

using Godot;

public partial class PlayerSpawnSettings : Resource {
  public Controller Controller { get; private set; }
  public PlayerState PlayerState { get; private set; }
  public string PlayerName { get; private set; }
  public HUD? HUD { get; private set; }
  public Node? CharacterNode { get; private set; }
  public PlayerSpawnSettings(Controller controller, PlayerState playerState, string playerName = "Player", HUD? hud = null, Node? character = null) {
    Controller = controller;
    PlayerState = playerState;
    PlayerName = playerName;
    HUD = hud;
    CharacterNode = character;
  }
}

[GlobalClass]
public partial class PlayerManagmentComponent : GameModeComponent {
  [Signal] public delegate void PlayerSpawnedEventHandler(PlayerState playerState);

  public override void _Ready() {
    base._Ready();

    GetGameState().PlayerArray = new();
  }

  public override IPlayerManagmentGameState GetGameState() => (base.GetGameState() as IPlayerManagmentGameState)!;

  public void SpawnPlayerDeferred(PlayerSpawnSettings spawnSettings) {
    CallDeferred(MethodName.SpawnPlayer, spawnSettings);
  }

  public virtual void SpawnPlayer(PlayerSpawnSettings settings) {
    var parent = GameInstance.GetWorld().GetCurrentLevel();

    if (settings.CharacterNode != null) {
      parent.AddChild(settings.CharacterNode);
      if (settings.Controller is PlayerController playerController) {
        if (settings.HUD != null) {
          playerController.SetHUD(settings.HUD);
          playerController.SpawnHUD();
        }

        if (settings.CharacterNode is IPlayerControllable controllable) {
          playerController.SetControllable(controllable);
        }
      }
    }

    var playerState = settings.PlayerState;
    playerState.SetPlayerName(settings.PlayerName);
    playerState.SetPlayerIndex(GetGameState().PlayerArray.Count);
    playerState.SetController(settings.Controller);

    settings.Controller.SetPlayerState(playerState);

    GetGameState().PlayerArray.Add(playerState);

    parent.AddChild(playerState);
    parent.AddChild(settings.Controller);

    EmitSignal(SignalName.PlayerSpawned, playerState);
  }
}
