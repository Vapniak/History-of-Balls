namespace HOB;

using Godot;

public partial class Main : Node {
  [Export] private PackedScene _splashScreenScene;

  public override void _Ready() {
    GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _splashScreenScene);
  }
}
