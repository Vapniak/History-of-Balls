namespace HOB;

using GameplayFramework;
using Godot;

public partial class Main : Node {
  public override void _Ready() {
    CallDeferred(MethodName.CreateWorld);
  }

  private void CreateWorld() {
    GameInstance.CreateWorld("splash_level");
  }
}
