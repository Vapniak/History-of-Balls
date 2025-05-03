namespace HOB;

using Godot;
using System;
using System.Threading.Tasks;
using WidgetSystem;

public partial class TutorialGameMode : ControlGameMode {
  public override void _Ready() {
    base._Ready();

    WidgetManager.Instance.PushWidget<TutorialMenuWidget>();
  }
}
