namespace GameplayFramework;

using Godot;

// maybe don't make it global node and just make it static instance
/// <summary>
/// Main game manager.
/// </summary>
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

  // TODO: get and set game mode


  public static PlayerState GetPlayerState(int index) {
    return Instance.World.GameMode.GameState.PlayerArray[index];
  }
  public static T GetPlayerState<T>(int index) where T : PlayerState {
    return GetPlayerState(index) as T;
  }
  public static void SetPlayerState(int index, PlayerState playerState) {
    Instance.World.GameMode.GameState.PlayerArray[index] = playerState;
  }

  public static World GetWorld() {
    return Instance.World;
  }
}
