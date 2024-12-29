namespace GameplayFramework;

using Godot;

[GlobalClass]
public sealed partial class Level : Node {
  [Export] private PackedScene GameModeScene { get; set; }
  internal GameMode GameMode { get; private set; }

  private bool _canChangeGameMode = true;


  /// <summary>
  /// Changes the level game mode. It can be changed until the level is loaded.
  /// </summary>
  /// <param name="gameMode"></param>
  public void ChangeGameMode(GameMode gameMode) {
    if (!_canChangeGameMode) {
      GD.PrintErr("Can't change game mode, because level is already loaded.");
      return;
    }

    GameMode = gameMode;
  }
  public void Load() {
    GameMode = GameModeScene.InstantiateOrNull<GameMode>();
    if (GameMode == null) {
      GD.PrintErr("Game Mode Scene is null");
      return;
    }

    _canChangeGameMode = false;
    AddChild(GameMode);
    GameMode.Init();
  }

  // TODO: level streaming, so you can add multiple levels but keep the same game mode

  public void UnLoad() {

  }

}
