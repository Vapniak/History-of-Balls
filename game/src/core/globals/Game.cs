namespace HOB.Core;

using Godot;

public partial class Game : Node {
  public static Game Instance { get; private set; }

  internal World World { get; set; }

  public override void _EnterTree() {
    if (Instance == null) {
      Instance = this;
    }
    else {
      QueueFree();
    }
  }

  public override void _ExitTree() {
    if (Instance == this) {
      Instance = null;
    }
  }

  public void ChangeWorld(World world) {
    var parent = Instance.World.GetParent();
    parent.RemoveChild(World);
    Instance.World = world;
    parent.AddChild(World);
  }

  public static GameState GetGameState() {
    return Instance.World.GameMode.GameState;
  }
  public static T GetGameState<T>() where T : GameState {
    return GetGameState() as T;
  }
  public static void SetGameState(GameState gameState) {
    Instance.World.GameMode.GameState = gameState;
  }

  // TODO: get and set game mode


  public static PlayerState GetPlayerState() {
    return Instance.World.GameMode.PlayerState;
  }
  public static T GetPlayerState<T>() where T : PlayerState {
    return GetPlayerState() as T;
  }
  public static void SetPlayerState(PlayerState playerState) {
    Instance.World.GameMode.PlayerState = playerState;
  }

  public static World GetWorld() {
    return Instance.World;
  }
}
