namespace GameplayFramework;

using Godot;

[GlobalClass]
public partial class Level : Node {
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

    if (IsInstanceValid(GameMode)) {
      EmitSignal(SignalName.GameModeChanged, GameMode, newGameMode);
      GameMode.QueueFree();
    }
    GameMode = newGameMode;
    AddChild(GameMode);
  }
  public virtual void Load() {
    var gameMode = GameModeScene?.InstantiateOrNull<GameMode>();
    if (gameMode == null) {
      GD.PrintErr("Game Mode Scene is null");
      return;
    }

    ChangeGameMode(gameMode);
    _canChangeGameMode = false;
  }

  // TODO: level streaming, so you can add multiple levels but keep the same game mode

  public virtual void UnLoad() {
    QueueFree();
  }
}
