namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class TestPlayerManagmentComponent : PlayerManagmentComponent {
  protected override PlayerState CreatePlayerState() {
    var state = new TestPlayerState();

    return state;
  }
}
