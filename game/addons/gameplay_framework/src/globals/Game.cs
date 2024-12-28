namespace GameplayFramework;

using Godot;

// maybe don't make it global node and just make it static instance
/// <summary>
/// Main game manager.
/// </summary>
public partial class Game : Node {
  // TODO: better flow of creation and deletion of nodes
  [Export(PropertyHint.Dir)] public string LevelsDirectoryPath { get; private set; }
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

  public static GameState GetGameState() {
    return Instance.World.GetGameMode().GetGameState();
  }
  public static T GetGameState<T>() where T : GameState {
    return GetGameState() as T;
  }

  public static World GetWorld() {
    return Instance.World;
  }

  public static GameMode GetGameMode() {
    return Instance.World.GetGameMode();
  }

  public static T GetGameMode<T>() where T : GameMode {
    return GetGameMode() as T;
  }

  public void PauseGame() {
    GetTree().Paused = !GetTree().Paused;
  }

  public void QuitGame() {
    GetTree().Quit();
  }
}
