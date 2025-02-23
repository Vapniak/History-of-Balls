namespace GameplayFramework;

using Godot;

// maybe don't make it global node and just make it static instance
/// <summary>
/// Main game manager.
/// </summary>
public partial class GameInstance : Node {
  // TODO: better flow of creation and deletion of nodes
  [Export(PropertyHint.Dir)] public string LevelsDirectoryPath { get; private set; }
  public static GameInstance Instance { get; private set; }

  private World World { get; set; }
  public override void _EnterTree() {
    ProcessMode = ProcessModeEnum.Always;

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

  public static IGameState GetGameState() {
    return GetGameMode().GetGameState();
  }
  public static T GetGameState<T>() where T : class, IGameState {
    return GetGameState() as T;
  }

  public static GameMode GetGameMode() {
    return GetWorld().GetGameMode();
  }

  public static T GetGameMode<T>() where T : GameMode {
    return GetGameMode() as T;
  }

  public static void CreateWorld(string startLevelName = null) {
    var world = new World() {
      Name = "World"
    };
    Instance.World = world;

    Instance.GetTree().CurrentScene.QueueFree();
    Instance.GetTree().Root.AddChild(world);
    Instance.GetTree().CurrentScene = world;

    if (startLevelName != null) {
      world.OpenLevel(startLevelName);
    }
  }

  public static World GetWorld() {
    return Instance.World;
  }


  public static void SetPause(bool value) {
    Instance.GetTree().Paused = value;
  }

  public static void QuitGame() {
    Instance.GetTree().Quit();
  }

  public override void _Process(double delta) {
    if (GetWorld().LoadingLevel) {
      GetWorld()?.ProcessThrededLevelLoad();
    }
  }
}
