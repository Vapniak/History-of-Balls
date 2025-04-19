namespace HOB;

using GameplayFramework;
using Godot;

public partial class Main : Node {
  public override void _Ready() {
    CallDeferred(MethodName.CreateWorld);
  }

  private static void CreateWorld() {
    GameInstance.CreateWorld("splash_level");
  }
}
