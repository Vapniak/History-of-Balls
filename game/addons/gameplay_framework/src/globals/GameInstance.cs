namespace GameplayFramework;

using System.Diagnostics;
using Godot;

// maybe don't make it global node and just make it static instance
/// <summary>
/// Main game manager.
/// </summary>
public partial class GameInstance : Node {
  [Export(PropertyHint.Dir)] public string? LevelsDirectoryPath { get; private set; }
  public static GameInstance? Instance { get; private set; }

  private World? World { get; set; }

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
  public static T? GetGameState<T>() where T : class, IGameState {
    return GetGameState() as T;
  }

  public static IGameMode GetGameMode() {
    return GetWorld().GetGameMode();
  }

  public static T? GetGameMode<T>() where T : GameMode {
    return GetGameMode() as T;
  }

  public static void CreateWorld(string? startLevelName = null) {
    var world = new World() {
      Name = "World"
    };

    if (Instance == null) {
      Debug.Assert(false, "Game instance is null");
      return;
    }

    Instance.World = world;

    Instance.GetTree().CurrentScene.QueueFree();
    Instance.GetTree().Root.AddChild(world);
    Instance.GetTree().CurrentScene = world;

    if (startLevelName != null) {
      _ = Instance.World.OpenLevel(startLevelName);
    }
  }

  public static World GetWorld() {
    return Instance!.World!;
  }


  public static void SetPause(bool value) {
    Instance!.GetTree().Paused = value;
  }

  public static void QuitGame() {
    Instance!.GetTree().Quit();
  }

  public IController? GetPlayerController() {
    if (GetGameState() is IPlayerManagmentGameState playerManagment) {
      return playerManagment.PlayerArray[0].GetController();
    }

    return null;
  }
}
