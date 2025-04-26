namespace HOB;

using AudioManager;
using Godot;

[GlobalClass]
public partial class ButtonWidget : BaseButtonWidget<ButtonWidget, Button> {
  [Export] public override Button Button { get; protected set; } = default!;

  // public override void _EnterTree() {
  //   if (!IsInstanceValid(Button)) {
  //     return;
  //   }

  //   var pressedCallable = Callable.From(() => SoundManager.Instance.Play("ui", "click"));
  //   var hoveredCallable = Callable.From(() => SoundManager.Instance.Play("ui", "hover"));
  //   if (!Button.IsConnected(Button.SignalName.Pressed, pressedCallable)) {
  //     Button.Connect(Button.SignalName.Pressed, pressedCallable);
  //   }

  //   if (!Button.IsConnected(Button.SignalName.MouseEntered, hoveredCallable)) {
  //     Button.Connect(Button.SignalName.MouseEntered, hoveredCallable);
  //   }
  // }
}
