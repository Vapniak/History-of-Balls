namespace GameplayFramework;

using Godot;

[GlobalClass]
public sealed partial class Level : Node {
  [Export] private PackedScene GameModeScene { get; set; }
  internal GameMode GameMode { get; private set; }
  public void Load() {
    GameMode = GameModeScene.InstantiateOrNull<GameMode>();
    if (GameMode == null) {
      GD.PrintErr("Game Mode Scene is null");
      return;
    }

    AddChild(GameMode);
    GameMode.Init();
  }
  public void UnLoad() {

  }

  // TODO: level streaming, so you can add multiple levels but keep the same game mode
}
