namespace GameplayFramework;

using Godot;

// maybe don't make it global node and just make it static instance
/// <summary>
/// Main game manager.
/// </summary>

// TODO: rename it to game instance
public partial class Game : Node {
  // TODO: better flow of creation and deletion of nodes
  [Export(PropertyHint.Dir)] public string LevelsDirectoryPath { get; private set; }
  public static Game Instance { get; private set; }

  private World World { get; set; }
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

  public static GameState GetGameState() {
    return Instance.World.GetGameMode().GetGameState();
  }
  public static T GetGameState<T>() where T : GameState {
    return GetGameState() as T;
  }

  public static GameMode GetGameMode() {
    return Instance.World.GetGameMode();
  }

  public static T GetGameMode<T>() where T : GameMode {
    return GetGameMode() as T;
  }

  public static void CreateWorld(string startLevelName = null) {
    var world = new World();
    Instance.World = world;

    Instance.GetTree().CurrentScene.Free();
    Instance.GetTree().Root.AddChild(world);
    Instance.GetTree().CurrentScene = world;

    if (startLevelName != null) {
      world.OpenLevel(startLevelName);
    }
  }

  public static World GetWorld() {
    return Instance.World;
  }


  public static void PauseGame() {
    Instance.GetTree().Paused = !Instance.GetTree().Paused;
  }

  public static void QuitGame() {
    Instance.GetTree().Quit();
  }
}
