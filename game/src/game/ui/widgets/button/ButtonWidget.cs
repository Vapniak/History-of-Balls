namespace HOB;

using Godot;

[GlobalClass]
public partial class ButtonWidget : BaseButtonWidget<ButtonWidget, Button> {
  [Export] public override Button Button { get; protected set; } = default!;
}
