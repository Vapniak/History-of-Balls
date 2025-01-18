namespace GameplayFramework;

using Godot;

[GlobalClass]
public sealed partial class Level : Node {
  [Signal] public delegate void LoadedEventHandler();
  [Signal] public delegate void UnloadedEventHandler();
  [Signal] public delegate void GameModeChangedEventHandler(GameMode old, GameMode @new);

  [Export] private PackedScene GameModeScene { get; set; }

  public GameMode GameMode { get; private set; }

  private bool _canChangeGameMode = true;


  /// <summary>
  /// Changes the level game mode. It can be changed until the level is loaded.
  /// </summary>
  /// <param name="newGameMode"></param>
  public void ChangeGameMode(GameMode newGameMode) {
    if (!_canChangeGameMode) {
      GD.PrintErr("Can't change game mode, because level is already loaded.");
      return;
    }

    if (GameMode != null) {
      EmitSignal(SignalName.GameModeChanged, GameMode, newGameMode);
      GameMode.QueueFree();
    }
    GameMode = newGameMode;
    AddChild(GameMode);
    GameMode.Init();
  }
  public void Load() {
    var gameMode = GameModeScene.InstantiateOrNull<GameMode>();
    if (gameMode == null) {
      GD.PrintErr("Game Mode Scene is null");
      return;
    }

    ChangeGameMode(gameMode);
    _canChangeGameMode = false;

    EmitSignal(SignalName.Loaded);
  }

  // TODO: level streaming, so you can add multiple levels but keep the same game mode

  public void UnLoad() {
    EmitSignal(SignalName.Unloaded);
  }
}
